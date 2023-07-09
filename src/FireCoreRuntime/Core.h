#pragma once

#include <Windows.h>
#include <stdio.h>

class Core
{
public:
    Core()
    {
        // Check the available Windows SDK versions
        if (IsWindowsSDKPresent(L"v10.0"))
        {
            // Windows 10 SDK is available
            printf("Windows 10 SDK is available.\n");
            // Add your code here to use the Windows 10 SDK
        }
        else if (IsWindowsSDKPresent(L"v8.0"))
        {
            // Windows 8 SDK is available
            printf("Windows 8 SDK is available.\n");
            // Add your code here to use the Windows 8 SDK
        }
        else
        {
            // Neither Windows 10 nor Windows 8 SDK is available
            printf("No supported Windows SDK version is available.\n");
            // Add your fallback code here for unsupported systems
        }
    }

private:
    bool IsWindowsSDKPresent(const wchar_t* version)
    {
        HKEY hKey;
        wchar_t keyPath[MAX_PATH];
        bool isPresent = false;

        // Construct the registry key path for the Windows SDK
        swprintf_s(keyPath, L"SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\%s", version);

        // Open the registry key
        if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, keyPath, 0, KEY_READ, &hKey) == ERROR_SUCCESS)
        {
            isPresent = true;
            RegCloseKey(hKey);
        }

        return isPresent;
    }
};