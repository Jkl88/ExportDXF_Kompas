#include "patch_dxf.h"

#include <algorithm>
#include <string>
#include <vector>
#include <windows.h>

namespace {

bool IsSpace(char c)
{
    return c == ' ' || c == '\t' || c == '\r';
}

std::string Trim(const std::string& s)
{
    size_t b = 0;
    while (b < s.size() && IsSpace(s[b]))
        ++b;
    size_t e = s.size();
    while (e > b && IsSpace(s[e - 1]))
        --e;
    return s.substr(b, e - b);
}

std::string FormatInt(const std::string& sample, int value)
{
    std::string text = std::to_string(value);
    if (!sample.empty() && sample.size() > text.size() && IsSpace(sample[0]))
        return std::string(sample.size() - text.size(), ' ') + text;
    return text;
}

bool HeaderRange(const std::vector<std::string>& lines, size_t& headerStart, size_t& headerEnd)
{
    headerStart = static_cast<size_t>(-1);
    headerEnd = static_cast<size_t>(-1);
    if (lines.size() < 4)
        return false;

    for (size_t i = 0; i + 3 < lines.size(); ++i)
    {
        if (Trim(lines[i]) == "0" && Trim(lines[i + 1]) == "SECTION"
            && Trim(lines[i + 2]) == "2" && Trim(lines[i + 3]) == "HEADER")
        {
            headerStart = i;
            break;
        }
    }
    if (headerStart == static_cast<size_t>(-1))
        return false;

    for (size_t i = headerStart + 4; i + 1 < lines.size(); ++i)
    {
        if (Trim(lines[i]) == "0" && Trim(lines[i + 1]) == "ENDSEC")
        {
            headerEnd = i;
            return true;
        }
    }
    return false;
}

bool SetHeaderInt(std::vector<std::string>& lines, size_t headerStart, size_t& headerEnd, const char* name, int value)
{
    for (size_t i = headerStart; i + 3 < headerEnd; ++i)
    {
        if (Trim(lines[i]) != "9" || Trim(lines[i + 1]) != name)
            continue;
        if (Trim(lines[i + 2]) != "70")
            return false;

        std::string formatted = FormatInt(lines[i + 3], value);
        if (lines[i + 3] == formatted)
            return false;
        lines[i + 3] = formatted;
        return true;
    }

    std::string g9 = "  9";
    std::string g70 = " 70";
    std::string sample = "     0";
    for (size_t i = headerStart; i < headerEnd; ++i)
    {
        if (Trim(lines[i]) == "9")
            g9 = lines[i];
        if (Trim(lines[i]) == "70" && i + 1 < headerEnd)
        {
            g70 = lines[i];
            sample = lines[i + 1];
        }
    }

    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(headerEnd), g9);
    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(headerEnd + 1), name);
    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(headerEnd + 2), g70);
    lines.insert(lines.begin() + static_cast<std::ptrdiff_t>(headerEnd + 3), FormatInt(sample, value));
    headerEnd += 4;
    return true;
}

bool EndsWithDxf(const wchar_t* path)
{
    if (!path)
        return false;
    const wchar_t* dot = wcsrchr(path, L'.');
    return dot && (_wcsicmp(dot, L".dxf") == 0 || _wcsicmp(dot, L".dwg") == 0);
}

} // namespace

bool PatchDxfUnitsToMillimeters(const wchar_t* path)
{
    if (!EndsWithDxf(path))
        return false;

    HANDLE file = CreateFileW(path, GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (file == INVALID_HANDLE_VALUE)
        return false;

    LARGE_INTEGER size{};
    if (!GetFileSizeEx(file, &size) || size.QuadPart <= 0 || size.QuadPart > 64LL * 1024 * 1024)
    {
        CloseHandle(file);
        return false;
    }

    std::string bytes(static_cast<size_t>(size.QuadPart), '\0');
    DWORD read = 0;
    BOOL ok = ReadFile(file, bytes.data(), static_cast<DWORD>(bytes.size()), &read, nullptr);
    CloseHandle(file);
    if (!ok || read != bytes.size())
        return false;

    if (bytes.size() >= 18 && bytes.compare(0, 18, "AutoCAD Binary DXF") == 0)
        return false;
    // Двоичный DWG начинается с "AC10". Текстовый DXF — с пробелов и "0".
    if (bytes.size() >= 4 && bytes.compare(0, 4, "AC10") == 0)
        return false;

    const std::string nl = bytes.find("\r\n") != std::string::npos ? "\r\n" : "\n";
    std::vector<std::string> lines;
    size_t pos = 0;
    while (pos <= bytes.size())
    {
        size_t next = bytes.find('\n', pos);
        if (next == std::string::npos)
        {
            lines.push_back(bytes.substr(pos));
            break;
        }
        size_t len = next - pos;
        if (len > 0 && bytes[next - 1] == '\r')
            --len;
        lines.push_back(bytes.substr(pos, len));
        pos = next + 1;
        if (pos == bytes.size())
        {
            lines.push_back("");
            break;
        }
    }

    size_t headerStart = 0;
    size_t headerEnd = 0;
    if (!HeaderRange(lines, headerStart, headerEnd))
        return false;

    bool changed = false;
    changed |= SetHeaderInt(lines, headerStart, headerEnd, "$INSUNITS", 4);
    changed |= SetHeaderInt(lines, headerStart, headerEnd, "$MEASUREMENT", 1);
    if (!changed)
        return false;

    std::string out;
    out.reserve(bytes.size() + 32);
    for (size_t i = 0; i < lines.size(); ++i)
    {
        if (i)
            out += nl;
        out += lines[i];
    }

    file = CreateFileW(path, GENERIC_WRITE, 0, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
    if (file == INVALID_HANDLE_VALUE)
        return false;
    DWORD written = 0;
    ok = WriteFile(file, out.data(), static_cast<DWORD>(out.size()), &written, nullptr);
    CloseHandle(file);
    return ok && written == out.size();
}
