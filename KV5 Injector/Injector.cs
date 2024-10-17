
using System.Diagnostics;
using static KV5_Injector.Native;

namespace KV5_Injector;

public static class Injector
{
    /* -- Credits for the methods used --
     *  https://v3rm.net/threads/how-to-bypass-roblox-dll-signature-checks.11684/
     *  https://github.com/0Zayn/Ballistic/blob/master/Ballistic_Injector/Source/EntryPoint/EntryPoint.cpp
     */

    /* -- Requirements --
     *  In the dll you must have this export
     * 
     *  #pragma region DLL_EXPORTS
     *  extern "C" __declspec(dllexport) LRESULT NextHook(int code, WPARAM wParam, LPARAM lParam)
     *  {
     *      return CallNextHookEx(nullptr, code, wParam, lParam);
     *  }
     *  #pragma endregion
     *  
     *  Also make sure you have dotnet 8 runtime installed
     *  https://dotnet.microsoft.com/en-us/download/dotnet/8.0
     */

    /* -- How it works --
     *  
     *  MAKE SURE ROBLOX IS OPEN
     *  
     *  When calling
     *  Inject(string process, string processName, string modulePath)
     *  you want these for the parameters
     * 
     *  process = "RobloxPlayerBeta"
     *  processName = "Roblox"
     *  modulePath = "modulefilename.dll"
     * 
     *  The module dll has to be in the same directory as the main application
     *  
     *  You are going to have to add your own checks to make sure roblox is open before injecting
     *  
     */

    // Made by Nitro

    public static bool Inject(string process, string processName, string modulePath)
    {
        IntPtr windowHandle = FindWindow(null, processName);

        if (windowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException($"{processName} window not found.");
        }

        int processId = GetProcessId(process);
        if (processId == 0)
        {
            throw new InvalidOperationException($"{process} process not found.");
        }

        var processHandle = OpenProcess((int)ProcessAccess.AllAccess, false, processId);
        if (processHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to open process.");
        }

        IntPtr wintrustModule = LoadLibraryEx("wintrust.dll", IntPtr.Zero, (uint)DllCharacteristics.DontResolveDllReferences);
        IntPtr winVerifyTrust = GetProcAddress(wintrustModule, "WinVerifyTrust");

        byte[] payload = [
            0x48, 0x31, 0xC0,   // xor rax, rax
            0x59,               // pop rcx
            0xFF, 0xE1          // jmp rcx
        ];

        if (!VirtualProtectEx(processHandle, winVerifyTrust, payload.Length, (uint)MemoryProtection.ExecuteReadWrite, out uint oldProtect))
        {
            throw new InvalidOperationException("Failed to change memory protection.");
        }

        if (!WriteProcessMemory(processHandle, winVerifyTrust, payload, payload.Length, out int bytesWritten))
        {
            throw new InvalidOperationException("Failed to write memory.");
        }

        VirtualProtectEx(processHandle, winVerifyTrust, payload.Length, (uint)MemoryProtection.ExecuteRead, out _);

        int threadId = GetWindowThreadProcessId(windowHandle, out _);

        IntPtr targetModule = LoadLibraryEx(modulePath, IntPtr.Zero, (uint)DllCharacteristics.DontResolveDllReferences);
        IntPtr dllExport = GetProcAddress(targetModule, "NextHook");

        if (dllExport == IntPtr.Zero || targetModule == IntPtr.Zero)
        {
            throw new InvalidOperationException("Failed to find module export.");
        }

        IntPtr hookHandle = SetWindowsHookEx((int)HookType.GetMessage, dllExport, targetModule, threadId);
        if (hookHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Module hook failed.");
        }

        if (!PostThreadMessage(threadId, (uint)WindowMessage.Null, IntPtr.Zero, IntPtr.Zero))
        {
            throw new InvalidOperationException("Failed to post thread message.");
        }

        return true;
    }

    private static int GetProcessId(string processName)
    {
        return Process.GetProcessesByName(processName).FirstOrDefault()?.Id ?? 0;
    }
}
