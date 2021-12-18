# KernelBypassSharp
C# Kernel Mode Driver to read and write memory in protected processes.
This project is based on my https://github.com/VollRagm/KernelSharp repository.
The hooked function this example uses has been shared publicly, so Anticheat solutions might detect it.

The most important part of the code is in [Program.cs](https://github.com/VollRagm/KernelBypassSharp/blob/main/KernelBypassSharp/Program.cs).

# Compiling
Clone this repository. Then run `nuget restore` to restore the required packages. Open the build.bat and fix the file paths.
ILCPATH is located at something like `C:\Users\username\.nuget\packages\runtime.win-x64.microsoft.dotnet.ilcompiler\7.0.0-alpha.1.21430.2`, ntoskrnl.lib is located in the WDK install path.
Run `x64 Native Tools Command Prompt for VS 2019`, cd into the project directory and run build.bat.
You can load the driver or map it with kdmapper.

# Usage
After mapping or loading the driver, run the UsermodeApp to test it.
If you want to build your own API, simply replicate the structs and logic in the UsermodeApp example.
