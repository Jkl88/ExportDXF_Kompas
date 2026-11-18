#include <windows.h>

// Указатели заполняет EnsureOriginal до перехода в штатную библиотеку.
extern "C" void* g_LoadKompasLibrary = nullptr;
extern "C" void* g_UnloadKompasLibrary = nullptr;

void InstallFileHooks();

namespace {

void EmptyExport()
{
}

HMODULE ThisModule()
{
    HMODULE module = nullptr;
    GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
        reinterpret_cast<LPCWSTR>(&ThisModule), &module);
    return module;
}

} // namespace

extern "C" void EnsureOriginal()
{
    if (g_LoadKompasLibrary)
        return;

    wchar_t path[MAX_PATH];
    DWORD n = GetModuleFileNameW(ThisModule(), path, MAX_PATH);
    if (n == 0 || n >= MAX_PATH)
    {
        g_LoadKompasLibrary = reinterpret_cast<void*>(EmptyExport);
        g_UnloadKompasLibrary = reinterpret_cast<void*>(EmptyExport);
        return;
    }

    wchar_t* slash = wcsrchr(path, L'\\');
    if (!slash)
    {
        g_LoadKompasLibrary = reinterpret_cast<void*>(EmptyExport);
        g_UnloadKompasLibrary = reinterpret_cast<void*>(EmptyExport);
        return;
    }
    wcscpy_s(slash + 1, MAX_PATH - static_cast<size_t>(slash + 1 - path), L"dwgdxfExp_ascon.rtw");

    HMODULE original = LoadLibraryW(path);
    if (original)
    {
        g_LoadKompasLibrary = reinterpret_cast<void*>(GetProcAddress(original, "LoadKompasLibrary"));
        g_UnloadKompasLibrary = reinterpret_cast<void*>(GetProcAddress(original, "UnloadKompasLibrary"));
    }
    if (!g_LoadKompasLibrary)
        g_LoadKompasLibrary = reinterpret_cast<void*>(EmptyExport);
    if (!g_UnloadKompasLibrary)
        g_UnloadKompasLibrary = reinterpret_cast<void*>(EmptyExport);

    InstallFileHooks();
}

extern "C" __declspec(dllexport) int DxfUnitsPatchInstalled()
{
    return 1;
}

BOOL WINAPI DllMain(HINSTANCE module, DWORD reason, LPVOID)
{
    if (reason == DLL_PROCESS_ATTACH)
        DisableThreadLibraryCalls(module);
    return TRUE;
}
