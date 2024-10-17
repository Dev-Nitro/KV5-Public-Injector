# Injector Instructions

## Requirements

To use the injector with your DLL, ensure you have the following requirements:

- The DLL must contain the following export:

    ```cpp
    #pragma region DLL_EXPORTS
    extern "C" __declspec(dllexport) LRESULT NextHook(int code, WPARAM wParam, LPARAM lParam)
    {
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }
    #pragma endregion
    ```

- .NET 8 runtime must be installed. You can download it from [this link](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

## How It Works

**Important:** Make sure Roblox is open before proceeding with the injection.

### Usage

When calling the `Inject` method:

```csharp
Inject(string process, string processName, string modulePath);
```

### Parameter Values

Use the following parameter values:

- `process` = `RobloxPlayerBeta`
- `processName` = `Roblox`
- `modulePath` = `modulefilename.dll`

### Notes

- Ensure that the module DLL is located in the same directory as the main application.
- You will need to implement your own checks to verify that Roblox is open before initiating the injection process.

Made by Nitro
