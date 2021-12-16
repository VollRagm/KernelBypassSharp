using System.Runtime;
using static KernelSharp.WDK;

namespace KernelBypassSharp
{
    public unsafe class Program
    {
        //Dummy main method to satisfy the compiler
        static void Main() { }

        [RuntimeExport("DriverEntry")]
        static NTSTATUS DriverEntry()
        {
            var win32k = Util.GetKernelModuleByName("win32kbase.sys");
            DbgPrintEx(0, 0, "win32kbase.sys -> %p", win32k);

            PVOID function = ((ulong)Util.FindPatternImage((byte*)win32k, "\x74\x20\x48\x8B\x44\x24\x00\x44", "xxxxxx?x"))- 0xA;
            DbgPrintEx(0, 0, "ApiSetEditionGetUserObjectInformationEntryPoint -> %p", function);

            return NTSTATUS.Success;
        }
    }
}
