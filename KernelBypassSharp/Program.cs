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
            DbgPrintEx(0, 0, "Win32k: %p", win32k);

            return NTSTATUS.Success;
        }
    }
}
