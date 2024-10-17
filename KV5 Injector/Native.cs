
using System.Runtime.InteropServices;

namespace KV5_Injector;

internal static class Native
{
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr OpenProcess(int processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

    [DllImport("kernel32.dll")]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, IntPtr lpfn, IntPtr hMod, int dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool PostThreadMessage(int idThread, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    internal enum ProcessAccess : int
    {
        AllAccess = 0x1F0FFF
    }

    internal enum MemoryProtection : uint
    {
        ExecuteReadWrite = 0x40,
        ExecuteRead = 0x20
    }

    internal enum HookType : int
    {
        GetMessage = 3
    }

    internal enum WindowMessage : uint
    {
        Null = 0x0000
    }

    internal enum DllCharacteristics : uint
    {
        DontResolveDllReferences = 0x00000001
    }

}
