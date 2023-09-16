#include <Windows.h>
#include <stdio.h>
#include <tchar.h>
#include <iostream>

// Function to check if a specific Windows SDK is available
bool IsWindowsSDKPresent(const wchar_t* version)
{
    HKEY hKey;
    wchar_t keyPath[MAX_PATH];
    bool isPresent = false;

    // Construct the registry key path for the Windows SDK
    _snwprintf_s(keyPath, _countof(keyPath), L"SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\%s", version);

    // Open the registry key
    if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, keyPath, 0, KEY_READ, &hKey) == ERROR_SUCCESS)
    {
        isPresent = true;
        RegCloseKey(hKey);
    }

    return isPresent;
}

// Function to run FireCoreRuntime.exe in the background
void RunFireCoreRuntime()
{
    STARTUPINFO si = { sizeof(STARTUPINFO) };
    PROCESS_INFORMATION pi;

    if (CreateProcess(L"C:\\Path\\To\\FireCoreRuntime.exe", nullptr, nullptr, nullptr, FALSE, CREATE_NO_WINDOW, nullptr, nullptr, &si, &pi))
    {
        CloseHandle(pi.hThread);
        CloseHandle(pi.hProcess);
        std::cout << "FireCoreRuntime.exe is running in the background." << std::endl;
    }
    else
    {
        std::cerr << "Failed to start FireCoreRuntime.exe." << std::endl;
    }
}

int main()
{
    // Check if Windows 10 SDK is present
    if (IsWindowsSDKPresent(L"v10.0"))
    {
        // Windows 10 SDK is available, reference it here
        // Add your code here to use the Windows 10 SDK
    }
    else if (IsWindowsSDKPresent(L"v8.0"))
    {
        // Windows 8 SDK is available, reference it here
        // Add your code here to use the Windows 8 SDK
    }
    else
    {
        // Neither Windows 10 nor Windows 8 SDK is available
        // Add your fallback code here for unsupported systems
    }

    // Run FireCoreRuntime.exe when the UWP app is being run
    // You can trigger this function based on specific conditions when the UWP app is running.

    return 0;
}
