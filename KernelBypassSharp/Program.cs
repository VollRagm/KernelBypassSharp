using System.Runtime;
using static KernelSharp.WDK;

public unsafe class Program
{
    //Dummy main method to satisfy the compiler
    static void Main() { }

    [RuntimeExport("DriverEntry")]
    static NTSTATUS DriverEntry()
    {
        return NTSTATUS.Success;
    }
}