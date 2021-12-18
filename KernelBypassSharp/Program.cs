using System.Runtime;
using System.Runtime.InteropServices;
using static KernelSharp.WDK;
using static KernelSharp.WDK.Undocumented;


namespace KernelBypassSharp
{
    public unsafe class Program
    {
        static delegate*<void*, void*, void*, void*, void*, ulong> NtUserGetObjectInformationOriginal;

        //Dummy main method to satisfy the compiler
        static void Main() { }


        static void HookHandler(SyscallData data)
        {
            if (data.ProcessId == 0) return;

            PEPROCESS proc = 0;
            if (!NT_SUCCESS(PsLookupProcessByProcessId(data.ProcessId, &proc)))
                return;

            if (proc == 0) return;

            ulong outSize = 0;
            switch (data.Op)
            {
                case Operation.Read:

                    //I could check NTSTATUS here, but this is up to you to implement
                    MmCopyVirtualMemory(proc, data.SourceAddress, IoGetCurrentProcess(), data.TargetAddress, data.Size, KProcessorMode.UserMode, &outSize);
                    break;

                case Operation.Write:

                    MmCopyVirtualMemory(IoGetCurrentProcess(), data.SourceAddress, proc, data.TargetAddress, data.Size, KProcessorMode.UserMode, &outSize);
                    break;

                case Operation.Base:

                    PVOID processBase = PsGetProcessSectionBaseAddress(proc);
                    *(PVOID*)data.TargetAddress = processBase;

                    break;
            }
        }


        //hook handler based on https://github.com/btbd/access/blob/noseh/Driver/main.c
        static ulong NtUserGetObjectInformationHook(void* a1, void* a2, SyscallData* data, ulong* status, void* a5)
        {
            if (ExGetPreviousMode() != KProcessorMode.UserMode)
                return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);

            SyscallData safeData = new SyscallData();
         
            //Check for magic and memory validity
            if(!Util.ProbeUserAddress(data, (ulong)sizeof(SyscallData), sizeof(uint)) || !Util.SafeCopy(&safeData, data, (ulong)sizeof(SyscallData)) || safeData.Magic != 0x69420)
                return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);

            HookHandler(safeData);

            return NtUserGetObjectInformationOriginal(a1, a2, data, status, a5);
        }



        [RuntimeExport("DriverEntry")]
        static NTSTATUS DriverEntry()
        {
            var win32k = Util.GetKernelModuleByName("win32kbase.sys");

            if (win32k == 0) return NTSTATUS.DllNotFound;

            PVOID function = ((ulong)Util.FindPatternImage((byte*)win32k, "\x74\x20\x48\x8B\x44\x24\x00\x44", "xxxxxx?x")) - 0xA;

            if (function == 0) return NTSTATUS.ProcedureNotFound;

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
            public uint ProcessId;
            public Operation Op;
            public PVOID SourceAddress;
            public PVOID TargetAddress;
            public ulong Size;
        }

        enum Operation
        {
            Read,
            Write,
            Base
        }
    }
}
