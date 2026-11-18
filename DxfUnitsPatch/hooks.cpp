#include "patch_dxf.h"

#include <atomic>
#include <mutex>
#include <string>
#include <unordered_map>
#include <vector>
#include <windows.h>
#include <psapi.h>

#pragma comment(lib, "psapi.lib")

namespace {

using CreateFileW_fn = HANDLE(WINAPI*)(LPCWSTR, DWORD, DWORD, LPSECURITY_ATTRIBUTES, DWORD, DWORD, HANDLE);
using CloseHandle_fn = BOOL(WINAPI*)(HANDLE);
using MoveFileExW_fn = BOOL(WINAPI*)(LPCWSTR, LPCWSTR, DWORD);
using MoveFileW_fn = BOOL(WINAPI*)(LPCWSTR, LPCWSTR);
using LoadLibraryW_fn = HMODULE(WINAPI*)(LPCWSTR);
using LoadLibraryExW_fn = HMODULE(WINAPI*)(LPCWSTR, HANDLE, DWORD);
using GetInsUnits_fn = int(__fastcall*)(void*);

CreateFileW_fn g_realCreateFileW = nullptr;
CloseHandle_fn g_realCloseHandle = nullptr;
MoveFileExW_fn g_realMoveFileExW = nullptr;
MoveFileW_fn g_realMoveFileW = nullptr;
LoadLibraryW_fn g_realLoadLibraryW = nullptr;
LoadLibraryExW_fn g_realLoadLibraryExW = nullptr;
GetInsUnits_fn g_realGetInsUnits = nullptr;

extern "C" void* g_writeTrampoline = nullptr;
extern "C" void HookWriteFile();

std::atomic<int> g_writeCount{ 0 };
thread_local int g_inPatch = 0;
std::mutex g_filesMutex;
std::unordered_map<HANDLE, std::wstring> g_files;
std::once_flag g_hookOnce;

bool EndsWithDxf(const wchar_t* path)
{
    if (!path)
        return false;
    const wchar_t* dot = wcsrchr(path, L'.');
    return dot && (_wcsicmp(dot, L".dxf") == 0 || _wcsicmp(dot, L".dwg") == 0);
}

void PatchLater(const wchar_t* path)
{
    if (!path || !EndsWithDxf(path))
        return;
    ++g_inPatch;
    try
    {
        PatchDxfUnitsToMillimeters(path);
    }
    catch (...)
    {
    }
    --g_inPatch;
}

// Слот переходника kernel32: FF 25 disp32 / jmp qword ptr [rip+disp].
void* HookKernel32Forwarder(const char* name, void* hook)
{
    HMODULE kernel = GetModuleHandleW(L"kernel32.dll");
    if (!kernel)
        return nullptr;
    BYTE* fn = reinterpret_cast<BYTE*>(GetProcAddress(kernel, name));
    if (!fn || fn[0] != 0xFF || fn[1] != 0x25)
        return nullptr;

    INT32 disp = *reinterpret_cast<INT32*>(fn + 2);
    void** slot = reinterpret_cast<void**>(fn + 6 + disp);
    void* original = *slot;
    DWORD old = 0;
    if (!VirtualProtect(slot, sizeof(void*), PAGE_READWRITE, &old))
        return nullptr;
    *slot = hook;
    VirtualProtect(slot, sizeof(void*), old, &old);
    FlushInstructionCache(GetCurrentProcess(), fn, 16);
    return original;
}

void PatchModuleIat(HMODULE module);

HANDLE WINAPI HookCreateFileW(LPCWSTR name, DWORD access, DWORD share, LPSECURITY_ATTRIBUTES sa,
    DWORD disp, DWORD flags, HANDLE templateFile)
{
    HANDLE handle = g_realCreateFileW(name, access, share, sa, disp, flags, templateFile);
    if (g_inPatch || handle == INVALID_HANDLE_VALUE || !EndsWithDxf(name))
        return handle;

    const bool writable = (access & (GENERIC_WRITE | GENERIC_ALL | FILE_APPEND_DATA)) != 0;
    if (!writable)
        return handle;

    wchar_t full[MAX_PATH];
    DWORD n = GetFullPathNameW(name, MAX_PATH, full, nullptr);
    {
        std::lock_guard<std::mutex> lock(g_filesMutex);
        g_files[handle] = (n > 0 && n < MAX_PATH) ? full : name;
    }
    g_writeCount.fetch_add(1);
    return handle;
}

BOOL WINAPI HookCloseHandle(HANDLE handle)
{
    std::wstring path;
    bool tracked = false;
    if (!g_inPatch && g_writeCount.load() > 0)
    {
        std::lock_guard<std::mutex> lock(g_filesMutex);
        auto it = g_files.find(handle);
        if (it != g_files.end())
        {
            path = it->second;
            g_files.erase(it);
            tracked = true;
        }
    }

    BOOL ok = g_realCloseHandle(handle);
    if (tracked)
    {
        g_writeCount.fetch_sub(1);
        if (ok)
            PatchLater(path.c_str());
    }
    return ok;
}

BOOL WINAPI HookMoveFileExW(LPCWSTR from, LPCWSTR to, DWORD flags)
{
    BOOL ok = g_realMoveFileExW(from, to, flags);
    if (ok && !g_inPatch)
        PatchLater(to);
    return ok;
}

BOOL WINAPI HookMoveFileW(LPCWSTR from, LPCWSTR to)
{
    BOOL ok = g_realMoveFileW(from, to);
    if (ok && !g_inPatch)
        PatchLater(to);
    return ok;
}

// Экспортёр читает единицы до открытия файла, поэтому дюймы (0 и 1)
// подменяются всегда, пока загружена библиотека экспорта.
int __fastcall HookGetInsUnits(void* self)
{
    int value = g_realGetInsUnits(self);
    if (value == 0 || value == 1)
        return 4;
    return value;
}

std::string TrimCopy(const std::string& s)
{
    size_t b = 0;
    while (b < s.size() && (s[b] == ' ' || s[b] == '\t' || s[b] == '\r'))
        ++b;
    size_t e = s.size();
    while (e > b && (s[e - 1] == ' ' || s[e - 1] == '\t' || s[e - 1] == '\r'))
        --e;
    return s.substr(b, e - b);
}

// Ловит $INSUNITS / $MEASUREMENT даже когда заголовок пишется построчно.
bool RewriteDxfChunk(const char* input, DWORD size, std::string& output)
{
    thread_local std::string carry;
    thread_local int phase = 0; // 1 INSUNITS, 2 после 70; 3 MEASUREMENT, 4 после 70

    if (phase == 0 && carry.empty() && (size > 1024 * 1024 || memchr(input, '$', size) == nullptr))
        return false;
    if (carry.size() > 512)
    {
        carry.clear();
        phase = 0;
    }

    const size_t oldCarry = carry.size();
    std::string all = carry;
    all.append(input, input + size);
    const size_t lastNl = all.rfind('\n');
    if (lastNl == std::string::npos)
    {
        carry = all;
        return false;
    }

    std::string modified = all;
    bool changed = false;
    size_t i = 0;
    while (i <= lastNl)
    {
        size_t eol = modified.find('\n', i);
        if (eol == std::string::npos || eol > lastNl)
            break;
        size_t lineEnd = eol;
        if (lineEnd > i && modified[lineEnd - 1] == '\r')
            --lineEnd;
        const std::string trim = TrimCopy(modified.substr(i, lineEnd - i));

        auto patchDigit = [&](char digit) {
            const size_t rel = modified.rfind(digit, lineEnd > i ? lineEnd - 1 : i);
            if (rel == std::string::npos || rel < i || rel >= lineEnd)
                return;
            if (rel >= oldCarry && rel < oldCarry + size)
            {
                modified[rel] = (phase == 2) ? '4' : '1';
                changed = true;
            }
        };

        if (trim == "$INSUNITS")
            phase = 1;
        else if (phase == 1 && trim == "70")
            phase = 2;
        else if (phase == 2 && trim == "1")
        {
            patchDigit('1');
            phase = 0;
        }
        else if (trim == "$MEASUREMENT")
            phase = 3;
        else if (phase == 3 && trim == "70")
            phase = 4;
        else if (phase == 4 && trim == "0")
        {
            patchDigit('0');
            phase = 0;
        }
        else if (!trim.empty())
            phase = 0;

        i = eol + 1;
    }

    carry = all.substr(lastNl + 1);
    if (!changed)
        return false;
    output.assign(modified, oldCarry, size);
    return true;
}

extern "C" void* MaybeRewrite(const void* buffer, unsigned size)
{
    thread_local std::string storage;
    if (g_inPatch || !buffer || size == 0 || size > 16u * 1024u * 1024u)
        return nullptr;
    if (!RewriteDxfChunk(static_cast<const char*>(buffer), size, storage))
        return nullptr;
    return storage.data();
}

void InstallWriteFileHook()
{
    if (g_writeTrampoline)
        return;

    HMODULE base = GetModuleHandleW(L"kernelbase.dll");
    if (!base)
        return;
    BYTE* fn = reinterpret_cast<BYTE*>(GetProcAddress(base, "WriteFile"));
    if (!fn)
        return;

    const BYTE expected[] = {
        0x48, 0x89, 0x5C, 0x24, 0x10,
        0x48, 0x89, 0x74, 0x24, 0x18,
        0x4C, 0x89, 0x4C, 0x24, 0x20
    };
    if (memcmp(fn, expected, sizeof(expected)) != 0)
        return;

    BYTE* trampoline = static_cast<BYTE*>(VirtualAlloc(nullptr, 64, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE));
    if (!trampoline)
        return;
    memcpy(trampoline, fn, sizeof(expected));
    trampoline[15] = 0xFF;
    trampoline[16] = 0x25;
    *reinterpret_cast<DWORD*>(trampoline + 17) = 0;
    *reinterpret_cast<void**>(trampoline + 21) = fn + sizeof(expected);
    g_writeTrampoline = trampoline;

    BYTE jmp[15] = { 0xFF, 0x25, 0, 0, 0, 0 };
    *reinterpret_cast<void**>(jmp + 6) = reinterpret_cast<void*>(HookWriteFile);
    jmp[14] = 0x90;
    DWORD old = 0;
    if (!VirtualProtect(fn, sizeof(jmp), PAGE_EXECUTE_READWRITE, &old))
        return;
    memcpy(fn, jmp, sizeof(jmp));
    VirtualProtect(fn, sizeof(jmp), old, &old);
    FlushInstructionCache(GetCurrentProcess(), fn, sizeof(jmp));
}

void TryHookGetInsUnits()
{
    if (g_realGetInsUnits)
        return;

    HMODULE db = GetModuleHandleW(L"TD_DbCore_24.1_17.dll");
    if (!db)
        return;

    BYTE* fn = reinterpret_cast<BYTE*>(GetProcAddress(db, "?getINSUNITS@OdDbDatabase@@UEBA?AW4UnitsValue@OdDb@@XZ"));
    if (!fn)
        return;

    const BYTE expected[] = { 0x48, 0x8B, 0x41, 0x10, 0x8B, 0x80, 0x70, 0x07, 0x00, 0x00, 0xC3 };
    if (memcmp(fn, expected, sizeof(expected)) != 0)
        return;

    BYTE* trampoline = static_cast<BYTE*>(VirtualAlloc(nullptr, 32, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE));
    if (!trampoline)
        return;
    memcpy(trampoline, fn, sizeof(expected));
    g_realGetInsUnits = reinterpret_cast<GetInsUnits_fn>(trampoline);

    BYTE jmp[14] = { 0xFF, 0x25, 0, 0, 0, 0 };
    *reinterpret_cast<void**>(jmp + 6) = reinterpret_cast<void*>(HookGetInsUnits);
    DWORD old = 0;
    if (!VirtualProtect(fn, sizeof(jmp), PAGE_EXECUTE_READWRITE, &old))
        return;
    memcpy(fn, jmp, sizeof(jmp));
    VirtualProtect(fn, sizeof(jmp), old, &old);
    FlushInstructionCache(GetCurrentProcess(), fn, sizeof(jmp));
}

HMODULE WINAPI HookLoadLibraryW(LPCWSTR name)
{
    HMODULE module = g_realLoadLibraryW(name);
    if (module)
    {
        PatchModuleIat(module);
        TryHookGetInsUnits();
    }
    return module;
}

HMODULE WINAPI HookLoadLibraryExW(LPCWSTR name, HANDLE file, DWORD flags)
{
    HMODULE module = g_realLoadLibraryExW(name, file, flags);
    if (module)
    {
        PatchModuleIat(module);
        TryHookGetInsUnits();
    }
    return module;
}

struct HookName
{
    const char* name;
    void* hook;
};

void PatchModuleIat(HMODULE module)
{
    if (!module || module == GetModuleHandleW(L"kernel32.dll") || module == GetModuleHandleW(L"kernelbase.dll"))
        return;

    BYTE* base = reinterpret_cast<BYTE*>(module);
    auto dos = reinterpret_cast<IMAGE_DOS_HEADER*>(base);
    if (dos->e_magic != IMAGE_DOS_SIGNATURE)
        return;
    auto nt = reinterpret_cast<IMAGE_NT_HEADERS64*>(base + dos->e_lfanew);
    if (nt->Signature != IMAGE_NT_SIGNATURE || nt->OptionalHeader.Magic != IMAGE_NT_OPTIONAL_HDR64_MAGIC)
        return;

    const auto& dir = nt->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT];
    if (!dir.VirtualAddress)
        return;

    HookName hooks[] = {
        { "CreateFileW", reinterpret_cast<void*>(HookCreateFileW) },
        { "CloseHandle", reinterpret_cast<void*>(HookCloseHandle) },
        { "MoveFileExW", reinterpret_cast<void*>(HookMoveFileExW) },
        { "MoveFileW", reinterpret_cast<void*>(HookMoveFileW) },
        { "LoadLibraryW", reinterpret_cast<void*>(HookLoadLibraryW) },
        { "LoadLibraryExW", reinterpret_cast<void*>(HookLoadLibraryExW) },
    };

    auto desc = reinterpret_cast<IMAGE_IMPORT_DESCRIPTOR*>(base + dir.VirtualAddress);
    for (; desc->Name; ++desc)
    {
        const char* dll = reinterpret_cast<const char*>(base + desc->Name);
        if (_stricmp(dll, "KERNEL32.dll") != 0 && _stricmp(dll, "KERNELBASE.dll") != 0)
            continue;

        auto realOf = [](const char* hooked) -> void* {
            if (strcmp(hooked, "CreateFileW") == 0) return reinterpret_cast<void*>(g_realCreateFileW);
            if (strcmp(hooked, "CloseHandle") == 0) return reinterpret_cast<void*>(g_realCloseHandle);
            if (strcmp(hooked, "MoveFileExW") == 0) return reinterpret_cast<void*>(g_realMoveFileExW);
            if (strcmp(hooked, "MoveFileW") == 0) return reinterpret_cast<void*>(g_realMoveFileW);
            if (strcmp(hooked, "LoadLibraryW") == 0) return reinterpret_cast<void*>(g_realLoadLibraryW);
            if (strcmp(hooked, "LoadLibraryExW") == 0) return reinterpret_cast<void*>(g_realLoadLibraryExW);
            return nullptr;
        };

        auto thunk = reinterpret_cast<IMAGE_THUNK_DATA64*>(base + desc->FirstThunk);
        auto orig = reinterpret_cast<IMAGE_THUNK_DATA64*>(base + (desc->OriginalFirstThunk ? desc->OriginalFirstThunk : desc->FirstThunk));
        for (; orig->u1.AddressOfData; ++orig, ++thunk)
        {
            if (IMAGE_SNAP_BY_ORDINAL64(orig->u1.Ordinal))
                continue;
            auto import = reinterpret_cast<IMAGE_IMPORT_BY_NAME*>(base + orig->u1.AddressOfData);
            for (const HookName& hook : hooks)
            {
                if (strcmp(import->Name, hook.name) != 0 || !realOf(hook.name))
                    continue;
                void** slot = reinterpret_cast<void**>(&thunk->u1.Function);
                if (*slot == hook.hook)
                    break;
                DWORD old = 0;
                if (VirtualProtect(slot, sizeof(void*), PAGE_READWRITE, &old))
                {
                    *slot = hook.hook;
                    VirtualProtect(slot, sizeof(void*), old, &old);
                }
                break;
            }
        }
    }
}

void PatchLoadedModules()
{
    std::vector<HMODULE> modules(256);
    DWORD needed = 0;
    if (!EnumProcessModules(GetCurrentProcess(), modules.data(), static_cast<DWORD>(modules.size() * sizeof(HMODULE)), &needed))
        return;
    size_t count = needed / sizeof(HMODULE);
    if (count > modules.size())
    {
        modules.resize(count);
        if (!EnumProcessModules(GetCurrentProcess(), modules.data(), static_cast<DWORD>(modules.size() * sizeof(HMODULE)), &needed))
            return;
        count = needed / sizeof(HMODULE);
    }
    for (size_t i = 0; i < count; ++i)
        PatchModuleIat(modules[i]);
}

} // namespace

void InstallFileHooks()
{
    std::call_once(g_hookOnce, [] {
        g_realCreateFileW = reinterpret_cast<CreateFileW_fn>(HookKernel32Forwarder("CreateFileW", reinterpret_cast<void*>(HookCreateFileW)));
        g_realCloseHandle = reinterpret_cast<CloseHandle_fn>(HookKernel32Forwarder("CloseHandle", reinterpret_cast<void*>(HookCloseHandle)));
        InstallWriteFileHook();
        g_realMoveFileExW = reinterpret_cast<MoveFileExW_fn>(HookKernel32Forwarder("MoveFileExW", reinterpret_cast<void*>(HookMoveFileExW)));
        g_realMoveFileW = reinterpret_cast<MoveFileW_fn>(HookKernel32Forwarder("MoveFileW", reinterpret_cast<void*>(HookMoveFileW)));
        g_realLoadLibraryW = reinterpret_cast<LoadLibraryW_fn>(HookKernel32Forwarder("LoadLibraryW", reinterpret_cast<void*>(HookLoadLibraryW)));
        g_realLoadLibraryExW = reinterpret_cast<LoadLibraryExW_fn>(HookKernel32Forwarder("LoadLibraryExW", reinterpret_cast<void*>(HookLoadLibraryExW)));
        PatchLoadedModules();
        TryHookGetInsUnits();
    });
}
