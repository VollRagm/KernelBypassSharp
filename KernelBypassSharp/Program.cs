using System.Runtime;
using System.Runtime.InteropServices;
using static KernelSharp.WDK;

namespace KernelBypassSharp
{
    public unsafe class Program
    {
        static delegate*<void*, void*, void*, void*, void*, ulong> NtUserGetObjectInformationOriginal;

        //Dummy main method to satisfy the compiler
        static void Main() { }


        //hook handler based on https://github.com/btbd/access/blob/noseh/Driver/main.c
        static ulong NtUserGetObjectInformationHook(void* a1, void* a2, SyscallData* data, ulong* status, void* a5)
        {
            if (ExGetPreviousMode() != KProcessorMode.UserMode)
                return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);

            SyscallData safeData = new SyscallData();
            if(!Util.ProbeUserAddress(data, (ulong)sizeof(SyscallData), sizeof(uint)) || !Util.SafeCopy(&safeData, data, (ulong)sizeof(SyscallData)) || safeData.Magic != 0x69420)
                return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);

            DbgPrintEx(0, 0, "Hook called!", 0);
            return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);
        }

        [RuntimeExport("DriverEntry")]
        static NTSTATUS DriverEntry()
        {
            var win32k = Util.GetKernelModuleByName("win32kbase.sys");
            DbgPrintEx(0, 0, "win32kbase.sys -> %p", win32k);

            PVOID function = ((ulong)Util.FindPatternImage((byte*)win32k, "\x74\x20\x48\x8B\x44\x24\x00\x44", "xxxxxx?x")) - 0xA;
            DbgPrintEx(0, 0, "ApiSetEditionGetUserObjectInformationEntryPoint -> %p", function);

            PVOID dataPtr = (byte*)function + *(int*)((byte*)function + 3) + 7;

            NtUserGetObjectInformationOriginal = (delegate*<void*, void*, void*, void*, void*, ulong>)*(ulong*)dataPtr;

            delegate*<void*, void*, SyscallData*, ulong*, void*, ulong> hookFunc = &NtUserGetObjectInformationHook;
            *(ulong*)dataPtr = (ulong)hookFunc;

            return NTSTATUS.Success;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SyscallData
        {
            public uint Magic;
        }
    }
}
