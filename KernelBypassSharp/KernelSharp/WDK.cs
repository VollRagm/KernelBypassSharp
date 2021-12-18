using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KernelSharp
{
    public static unsafe class WDK
    {
        #region Struct Definitions

        public struct PEPROCESS
        {
            private void* _Value;

            public static implicit operator PEPROCESS(void* value)
            {
                return new PEPROCESS { _Value = value };
            }

            public static implicit operator PEPROCESS(ulong value)
            {
                return new PEPROCESS { _Value = (void*)value };
            }

            public static implicit operator void*(PEPROCESS value)
            {
                return value._Value;
            }

            public static implicit operator ulong(PEPROCESS value)
            {
                return (ulong)value._Value;
            }
        }

        public struct PVOID
        {
            private void* _Value;

            public static implicit operator PVOID(void* value)
            {
                return new PVOID { _Value = value };
            }

            public static implicit operator PVOID(ulong value)
            {
                return new PVOID { _Value = (void*)value };
            }

            public static implicit operator void*(PVOID value)
            {
                return value._Value;
            }

            public static implicit operator ulong(PVOID value)
            {
                return (ulong)value._Value;
            }
        }

        #endregion

        #region Helper Methods

        public static bool NT_SUCCESS(NTSTATUS status)
        {
            return (((int)(status)) >= 0);
        }

        public static char* w_str(this string str)
        {
            fixed (char* wc = str)
                return wc;
        }

        /// <summary>
        /// Converts the Wide string into a Multibyte string. The pool has to be freed after usage.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static char* c_str(this string str)
        {
            fixed (void* wc = str)
            {
                //Allocate pool for char* taking the null terminator into consideration
                var buf = ExAllocatePool(PoolType.NonPagedPool, (ulong)str.Length + 1);

                //convert wchar_t* to char*
                wcstombs((char*)buf, wc, (ulong)(str.Length * 2) + 2);
                return (char*)buf;
            }
        }

        public static ulong __readcr3()
        {
            void* buffer = stackalloc byte[0x5C0];
            var sat = KeSaveStateForHibernate(buffer);
            ulong cr3 = *(ulong*)((ulong)buffer + 0x10);
            return cr3;
        }

        public static ulong DbgPrintEx(uint ComponentId, uint level, string Format, PVOID vararg1)
        {
            fixed (void* wc = Format)
            {
                //Allocate memory on the stack for char* taking the null terminator into consideration
                var buf = stackalloc byte[Format.Length + 1];

                //convert wchar_t* to char*
                wcstombs((char*)buf, wc, (ulong)(Format.Length * 2) + 2);
                return _DbgPrintEx(ComponentId, level, (char*)buf, vararg1);
            }
        }

        #endregion

        #region NTAPI Imports

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "ExAllocatePool")]
        public static extern PVOID ExAllocatePool(PoolType poolType, ulong size);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "ExFreePool")]
        public static extern void ExFreePool(PVOID pool);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "KeSaveStateForHibernate")]
        public static extern NTSTATUS KeSaveStateForHibernate(void* state);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "DbgPrintEx")]
        private static extern ulong _DbgPrintEx(uint ComponentId, uint level, char* Format, void* vararg1);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "ExGetPreviousMode")]
        public static extern KProcessorMode ExGetPreviousMode();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "IoGetCurrentProcess")]
        public static extern void* IoGetCurrentProcess();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "PsLookupProcessByProcessId")]
        public static extern NTSTATUS PsLookupProcessByProcessId(uint ProcessId, PEPROCESS* process);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "DbgBreakPoint")]
        public static extern void DbgBreakPoint();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "wcstombs")]
        public static extern ulong wcstombs(char* mbstr, void* wcstr, ulong count);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "strstr")]
        public static extern char* strstr(char* str, char* subStr);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "strlen")]
        public static extern ulong strlen(char* str);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "wcslen")]
        public static extern ulong wcslen(char* str);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [RuntimeImport("ntoskrnl.exe", "memcmp")]
        public static extern int memcmp(void* buf1, void* buf2, ulong size);



        #endregion

        public static class Undocumented
        {

            [MethodImpl(MethodImplOptions.InternalCall)]
            [RuntimeImport("ntoskrnl.exe", "ZwQuerySystemInformation")]
            public static extern NTSTATUS ZwQuerySystemInformation(SystemInformationClass SystemInformationClass, void* systemInformation, uint systemInformationLength, uint* ReturnLength);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [RuntimeImport("ntoskrnl.exe", "MmCopyVirtualMemory")]
            public static extern NTSTATUS MmCopyVirtualMemory(void* SourceProcess, void* SourceAddress, void* TargetProcess, void* TargetAddress, ulong BufferSize, KProcessorMode PreviousMode, ulong* ReturnSize);

            [MethodImpl(MethodImplOptions.InternalCall)]
            [RuntimeImport("ntoskrnl.exe", "PsGetProcessSectionBaseAddress")]
            public static extern PVOID PsGetProcessSectionBaseAddress(void* process);

            [StructLayout(LayoutKind.Sequential)]
            public struct RTL_PROCESS_MODULE_INFORMATION
            {
                public ulong Handle;
                public ulong MappedBase;
                public ulong ImageBase;
                public uint ImageSize;
                public uint Flags;
                public ushort LoadOrderIndex;
                public ushort InitOrderIndex;
                public ushort LoadCount;
                public ushort OffsetToFileName;
                public fixed byte FullPathName[256];
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RTL_PROCESS_MODULES
            {
                public uint NumberOfModules;
                public RTL_PROCESS_MODULE_INFORMATION Modules;
            }
        }

        #region Enums

        public enum KProcessorMode
        {
            KernelMode,
            UserMode,
            MaximumMode
        };

        public enum SystemInformationClass
        {
            SystemBasicInformation = 0x0,
            SystemProcessorInformation = 0x1,
            SystemPerformanceInformation = 0x2,
            SystemTimeOfDayInformation = 0x3,
            SystemPathInformation = 0x4,
            SystemProcessInformation = 0x5,
            SystemCallCountInformation = 0x6,
            SystemDeviceInformation = 0x7,
            SystemProcessorPerformanceInformation = 0x8,
            SystemFlagsInformation = 0x9,
            SystemCallTimeInformation = 0xa,
            SystemModuleInformation = 0xb,
            SystemLocksInformation = 0xc,
            SystemStackTraceInformation = 0xd,
            SystemPagedPoolInformation = 0xe,
            SystemNonPagedPoolInformation = 0xf,
            SystemHandleInformation = 0x10,
            SystemObjectInformation = 0x11,
            SystemPageFileInformation = 0x12,
            SystemVdmInstemulInformation = 0x13,
            SystemVdmBopInformation = 0x14,
            SystemFileCacheInformation = 0x15,
            SystemPoolTagInformation = 0x16,
            SystemInterruptInformation = 0x17,
            SystemDpcBehaviorInformation = 0x18,
            SystemFullMemoryInformation = 0x19,
            SystemLoadGdiDriverInformation = 0x1a,
            SystemUnloadGdiDriverInformation = 0x1b,
            SystemTimeAdjustmentInformation = 0x1c,
            SystemSummaryMemoryInformation = 0x1d,
            SystemMirrorMemoryInformation = 0x1e,
            SystemPerformanceTraceInformation = 0x1f,
            SystemObsolete0 = 0x20,
            SystemExceptionInformation = 0x21,
            SystemCrashDumpStateInformation = 0x22,
            SystemKernelDebuggerInformation = 0x23,
            SystemContextSwitchInformation = 0x24,
            SystemRegistryQuotaInformation = 0x25,
            SystemExtendServiceTableInformation = 0x26,
            SystemPrioritySeperation = 0x27,
            SystemVerifierAddDriverInformation = 0x28,
            SystemVerifierRemoveDriverInformation = 0x29,
            SystemProcessorIdleInformation = 0x2a,
            SystemLegacyDriverInformation = 0x2b,
            SystemCurrentTimeZoneInformation = 0x2c,
            SystemLookasideInformation = 0x2d,
            SystemTimeSlipNotification = 0x2e,
            SystemSessionCreate = 0x2f,
            SystemSessionDetach = 0x30,
            SystemSessionInformation = 0x31,
            SystemRangeStartInformation = 0x32,
            SystemVerifierInformation = 0x33,
            SystemVerifierThunkExtend = 0x34,
            SystemSessionProcessInformation = 0x35,
            SystemLoadGdiDriverInSystemSpace = 0x36,
            SystemNumaProcessorMap = 0x37,
            SystemPrefetcherInformation = 0x38,
            SystemExtendedProcessInformation = 0x39,
            SystemRecommendedSharedDataAlignment = 0x3a,
            SystemComPlusPackage = 0x3b,
            SystemNumaAvailableMemory = 0x3c,
            SystemProcessorPowerInformation = 0x3d,
            SystemEmulationBasicInformation = 0x3e,
            SystemEmulationProcessorInformation = 0x3f,
            SystemExtendedHandleInformation = 0x40,
            SystemLostDelayedWriteInformation = 0x41,
            SystemBigPoolInformation = 0x42,
            SystemSessionPoolTagInformation = 0x43,
            SystemSessionMappedViewInformation = 0x44,
            SystemHotpatchInformation = 0x45,
            SystemObjectSecurityMode = 0x46,
            SystemWatchdogTimerHandler = 0x47,
            SystemWatchdogTimerInformation = 0x48,
            SystemLogicalProcessorInformation = 0x49,
            SystemWow64SharedInformationObsolete = 0x4a,
            SystemRegisterFirmwareTableInformationHandler = 0x4b,
            SystemFirmwareTableInformation = 0x4c,
            SystemModuleInformationEx = 0x4d,
            SystemVerifierTriageInformation = 0x4e,
            SystemSuperfetchInformation = 0x4f,
            SystemMemoryListInformation = 0x50,
            SystemFileCacheInformationEx = 0x51,
            SystemThreadPriorityClientIdInformation = 0x52,
            SystemProcessorIdleCycleTimeInformation = 0x53,
            SystemVerifierCancellationInformation = 0x54,
            SystemProcessorPowerInformationEx = 0x55,
            SystemRefTraceInformation = 0x56,
            SystemSpecialPoolInformation = 0x57,
            SystemProcessIdInformation = 0x58,
            SystemErrorPortInformation = 0x59,
            SystemBootEnvironmentInformation = 0x5a,
            SystemHypervisorInformation = 0x5b,
            SystemVerifierInformationEx = 0x5c,
            SystemTimeZoneInformation = 0x5d,
            SystemImageFileExecutionOptionsInformation = 0x5e,
            SystemCoverageInformation = 0x5f,
            SystemPrefetchPatchInformation = 0x60,
            SystemVerifierFaultsInformation = 0x61,
            SystemSystemPartitionInformation = 0x62,
            SystemSystemDiskInformation = 0x63,
            SystemProcessorPerformanceDistribution = 0x64,
            SystemNumaProximityNodeInformation = 0x65,
            SystemDynamicTimeZoneInformation = 0x66,
            SystemCodeIntegrityInformation = 0x67,
            SystemProcessorMicrocodeUpdateInformation = 0x68,
            SystemProcessorBrandString = 0x69,
            SystemVirtualAddressInformation = 0x6a,
            SystemLogicalProcessorAndGroupInformation = 0x6b,
            SystemProcessorCycleTimeInformation = 0x6c,
            SystemStoreInformation = 0x6d,
            SystemRegistryAppendString = 0x6e,
            SystemAitSamplingValue = 0x6f,
            SystemVhdBootInformation = 0x70,
            SystemCpuQuotaInformation = 0x71,
            SystemNativeBasicInformation = 0x72,
            SystemErrorPortTimeouts = 0x73,
            SystemLowPriorityIoInformation = 0x74,
            SystemBootEntropyInformation = 0x75,
            SystemVerifierCountersInformation = 0x76,
            SystemPagedPoolInformationEx = 0x77,
            SystemSystemPtesInformationEx = 0x78,
            SystemNodeDistanceInformation = 0x79,
            SystemAcpiAuditInformation = 0x7a,
            SystemBasicPerformanceInformation = 0x7b,
            SystemQueryPerformanceCounterInformation = 0x7c,
            SystemSessionBigPoolInformation = 0x7d,
            SystemBootGraphicsInformation = 0x7e,
            SystemScrubPhysicalMemoryInformation = 0x7f,
            SystemBadPageInformation = 0x80,
            SystemProcessorProfileControlArea = 0x81,
            SystemCombinePhysicalMemoryInformation = 0x82,
            SystemEntropyInterruptTimingInformation = 0x83,
            SystemConsoleInformation = 0x84,
            SystemPlatformBinaryInformation = 0x85,
            SystemThrottleNotificationInformation = 0x86,
            SystemHypervisorProcessorCountInformation = 0x87,
            SystemDeviceDataInformation = 0x88,
            SystemDeviceDataEnumerationInformation = 0x89,
            SystemMemoryTopologyInformation = 0x8a,
            SystemMemoryChannelInformation = 0x8b,
            SystemBootLogoInformation = 0x8c,
            SystemProcessorPerformanceInformationEx = 0x8d,
            SystemSpare0 = 0x8e,
            SystemSecureBootPolicyInformation = 0x8f,
            SystemPageFileInformationEx = 0x90,
            SystemSecureBootInformation = 0x91,
            SystemEntropyInterruptTimingRawInformation = 0x92,
            SystemPortableWorkspaceEfiLauncherInformation = 0x93,
            SystemFullProcessInformation = 0x94,
            SystemKernelDebuggerInformationEx = 0x95,
            SystemBootMetadataInformation = 0x96,
            SystemSoftRebootInformation = 0x97,
            SystemElamCertificateInformation = 0x98,
            SystemOfflineDumpConfigInformation = 0x99,
            SystemProcessorFeaturesInformation = 0x9a,
            SystemRegistryReconciliationInformation = 0x9b,
            SystemSupportedProcessArchitectures = 0xb5,
        }

        public enum PoolType
        {
            NonPagedPool,
            NonPagedPoolExecute = NonPagedPool,
            PagedPool,
            NonPagedPoolMustSucceed = NonPagedPool + 2,
            DontUseThisType,
            NonPagedPoolCacheAligned = NonPagedPool + 4,
            PagedPoolCacheAligned,
            NonPagedPoolCacheAlignedMustS = NonPagedPool + 6,
            MaxPoolType,
            NonPagedPoolBase = 0,
            NonPagedPoolBaseMustSucceed = NonPagedPoolBase + 2,
            NonPagedPoolBaseCacheAligned = NonPagedPoolBase + 4,
            NonPagedPoolBaseCacheAlignedMustS = NonPagedPoolBase + 6,
            NonPagedPoolSession = 32,
            PagedPoolSession = NonPagedPoolSession + 1,
            NonPagedPoolMustSucceedSession = PagedPoolSession + 1,
            DontUseThisTypeSession = NonPagedPoolMustSucceedSession + 1,
            NonPagedPoolCacheAlignedSession = DontUseThisTypeSession + 1,
            PagedPoolCacheAlignedSession = NonPagedPoolCacheAlignedSession + 1,
            NonPagedPoolCacheAlignedMustSSession = PagedPoolCacheAlignedSession + 1,
            NonPagedPoolNx = 512,
            NonPagedPoolNxCacheAligned = NonPagedPoolNx + 4,
            NonPagedPoolSessionNx = NonPagedPoolNx + 32,
        }

        public enum NTSTATUS : uint
        {
            // Success
            Success = 0x00000000,
            Wait0 = 0x00000000,
            Wait1 = 0x00000001,
            Wait2 = 0x00000002,
            Wait3 = 0x00000003,
            Wait63 = 0x0000003f,
            Abandoned = 0x00000080,
            AbandonedWait0 = 0x00000080,
            AbandonedWait1 = 0x00000081,
            AbandonedWait2 = 0x00000082,
            AbandonedWait3 = 0x00000083,
            AbandonedWait63 = 0x000000bf,
            UserApc = 0x000000c0,
            KernelApc = 0x00000100,
            Alerted = 0x00000101,
            Timeout = 0x00000102,
            Pending = 0x00000103,
            Reparse = 0x00000104,
            MoreEntries = 0x00000105,
            NotAllAssigned = 0x00000106,
            SomeNotMapped = 0x00000107,
            OpLockBreakInProgress = 0x00000108,
            VolumeMounted = 0x00000109,
            RxActCommitted = 0x0000010a,
            NotifyCleanup = 0x0000010b,
            NotifyEnumDir = 0x0000010c,
            NoQuotasForAccount = 0x0000010d,
            PrimaryTransportConnectFailed = 0x0000010e,
            PageFaultTransition = 0x00000110,
            PageFaultDemandZero = 0x00000111,
            PageFaultCopyOnWrite = 0x00000112,
            PageFaultGuardPage = 0x00000113,
            PageFaultPagingFile = 0x00000114,
            CrashDump = 0x00000116,
            ReparseObject = 0x00000118,
            NothingToTerminate = 0x00000122,
            ProcessNotInJob = 0x00000123,
            ProcessInJob = 0x00000124,
            ProcessCloned = 0x00000129,
            FileLockedWithOnlyReaders = 0x0000012a,
            FileLockedWithWriters = 0x0000012b,

            // Informational
            Informational = 0x40000000,
            ObjectNameExists = 0x40000000,
            ThreadWasSuspended = 0x40000001,
            WorkingSetLimitRange = 0x40000002,
            ImageNotAtBase = 0x40000003,
            RegistryRecovered = 0x40000009,

            // Warning
            Warning = 0x80000000,
            GuardPageViolation = 0x80000001,
            DatatypeMisalignment = 0x80000002,
            Breakpoint = 0x80000003,
            SingleStep = 0x80000004,
            BufferOverflow = 0x80000005,
            NoMoreFiles = 0x80000006,
            HandlesClosed = 0x8000000a,
            PartialCopy = 0x8000000d,
            DeviceBusy = 0x80000011,
            InvalidEaName = 0x80000013,
            EaListInconsistent = 0x80000014,
            NoMoreEntries = 0x8000001a,
            LongJump = 0x80000026,
            DllMightBeInsecure = 0x8000002b,

            // Error
            Error = 0xc0000000,
            Unsuccessful = 0xc0000001,
            NotImplemented = 0xc0000002,
            InvalidInfoClass = 0xc0000003,
            InfoLengthMismatch = 0xc0000004,
            AccessViolation = 0xc0000005,
            InPageError = 0xc0000006,
            PagefileQuota = 0xc0000007,
            InvalidHandle = 0xc0000008,
            BadInitialStack = 0xc0000009,
            BadInitialPc = 0xc000000a,
            InvalidCid = 0xc000000b,
            TimerNotCanceled = 0xc000000c,
            InvalidParameter = 0xc000000d,
            NoSuchDevice = 0xc000000e,
            NoSuchFile = 0xc000000f,
            InvalidDeviceRequest = 0xc0000010,
            EndOfFile = 0xc0000011,
            WrongVolume = 0xc0000012,
            NoMediaInDevice = 0xc0000013,
            NoMemory = 0xc0000017,
            NotMappedView = 0xc0000019,
            UnableToFreeVm = 0xc000001a,
            UnableToDeleteSection = 0xc000001b,
            IllegalInstruction = 0xc000001d,
            AlreadyCommitted = 0xc0000021,
            AccessDenied = 0xc0000022,
            BufferTooSmall = 0xc0000023,
            ObjectTypeMismatch = 0xc0000024,
            NonContinuableException = 0xc0000025,
            BadStack = 0xc0000028,
            NotLocked = 0xc000002a,
            NotCommitted = 0xc000002d,
            InvalidParameterMix = 0xc0000030,
            ObjectNameInvalid = 0xc0000033,
            ObjectNameNotFound = 0xc0000034,
            ObjectNameCollision = 0xc0000035,
            ObjectPathInvalid = 0xc0000039,
            ObjectPathNotFound = 0xc000003a,
            ObjectPathSyntaxBad = 0xc000003b,
            DataOverrun = 0xc000003c,
            DataLate = 0xc000003d,
            DataError = 0xc000003e,
            CrcError = 0xc000003f,
            SectionTooBig = 0xc0000040,
            PortConnectionRefused = 0xc0000041,
            InvalidPortHandle = 0xc0000042,
            SharingViolation = 0xc0000043,
            QuotaExceeded = 0xc0000044,
            InvalidPageProtection = 0xc0000045,
            MutantNotOwned = 0xc0000046,
            SemaphoreLimitExceeded = 0xc0000047,
            PortAlreadySet = 0xc0000048,
            SectionNotImage = 0xc0000049,
            SuspendCountExceeded = 0xc000004a,
            ThreadIsTerminating = 0xc000004b,
            BadWorkingSetLimit = 0xc000004c,
            IncompatibleFileMap = 0xc000004d,
            SectionProtection = 0xc000004e,
            EasNotSupported = 0xc000004f,
            EaTooLarge = 0xc0000050,
            NonExistentEaEntry = 0xc0000051,
            NoEasOnFile = 0xc0000052,
            EaCorruptError = 0xc0000053,
            FileLockConflict = 0xc0000054,
            LockNotGranted = 0xc0000055,
            DeletePending = 0xc0000056,
            CtlFileNotSupported = 0xc0000057,
            UnknownRevision = 0xc0000058,
            RevisionMismatch = 0xc0000059,
            InvalidOwner = 0xc000005a,
            InvalidPrimaryGroup = 0xc000005b,
            NoImpersonationToken = 0xc000005c,
            CantDisableMandatory = 0xc000005d,
            NoLogonServers = 0xc000005e,
            NoSuchLogonSession = 0xc000005f,
            NoSuchPrivilege = 0xc0000060,
            PrivilegeNotHeld = 0xc0000061,
            InvalidAccountName = 0xc0000062,
            UserExists = 0xc0000063,
            NoSuchUser = 0xc0000064,
            GroupExists = 0xc0000065,
            NoSuchGroup = 0xc0000066,
            MemberInGroup = 0xc0000067,
            MemberNotInGroup = 0xc0000068,
            LastAdmin = 0xc0000069,
            WrongPassword = 0xc000006a,
            IllFormedPassword = 0xc000006b,
            PasswordRestriction = 0xc000006c,
            LogonFailure = 0xc000006d,
            AccountRestriction = 0xc000006e,
            InvalidLogonHours = 0xc000006f,
            InvalidWorkstation = 0xc0000070,
            PasswordExpired = 0xc0000071,
            AccountDisabled = 0xc0000072,
            NoneMapped = 0xc0000073,
            TooManyLuidsRequested = 0xc0000074,
            LuidsExhausted = 0xc0000075,
            InvalidSubAuthority = 0xc0000076,
            InvalidAcl = 0xc0000077,
            InvalidSid = 0xc0000078,
            InvalidSecurityDescr = 0xc0000079,
            ProcedureNotFound = 0xc000007a,
            InvalidImageFormat = 0xc000007b,
            NoToken = 0xc000007c,
            BadInheritanceAcl = 0xc000007d,
            RangeNotLocked = 0xc000007e,
            DiskFull = 0xc000007f,
            ServerDisabled = 0xc0000080,
            ServerNotDisabled = 0xc0000081,
            TooManyGuidsRequested = 0xc0000082,
            GuidsExhausted = 0xc0000083,
            InvalidIdAuthority = 0xc0000084,
            AgentsExhausted = 0xc0000085,
            InvalidVolumeLabel = 0xc0000086,
            SectionNotExtended = 0xc0000087,
            NotMappedData = 0xc0000088,
            ResourceDataNotFound = 0xc0000089,
            ResourceTypeNotFound = 0xc000008a,
            ResourceNameNotFound = 0xc000008b,
            ArrayBoundsExceeded = 0xc000008c,
            FloatDenormalOperand = 0xc000008d,
            FloatDivideByZero = 0xc000008e,
            FloatInexactResult = 0xc000008f,
            FloatInvalidOperation = 0xc0000090,
            FloatOverflow = 0xc0000091,
            FloatStackCheck = 0xc0000092,
            FloatUnderflow = 0xc0000093,
            IntegerDivideByZero = 0xc0000094,
            IntegerOverflow = 0xc0000095,
            PrivilegedInstruction = 0xc0000096,
            TooManyPagingFiles = 0xc0000097,
            FileInvalid = 0xc0000098,
            InstanceNotAvailable = 0xc00000ab,
            PipeNotAvailable = 0xc00000ac,
            InvalidPipeState = 0xc00000ad,
            PipeBusy = 0xc00000ae,
            IllegalFunction = 0xc00000af,
            PipeDisconnected = 0xc00000b0,
            PipeClosing = 0xc00000b1,
            PipeConnected = 0xc00000b2,
            PipeListening = 0xc00000b3,
            InvalidReadMode = 0xc00000b4,
            IoTimeout = 0xc00000b5,
            FileForcedClosed = 0xc00000b6,
            ProfilingNotStarted = 0xc00000b7,
            ProfilingNotStopped = 0xc00000b8,
            NotSameDevice = 0xc00000d4,
            FileRenamed = 0xc00000d5,
            CantWait = 0xc00000d8,
            PipeEmpty = 0xc00000d9,
            CantTerminateSelf = 0xc00000db,
            InternalError = 0xc00000e5,
            InvalidParameter1 = 0xc00000ef,
            InvalidParameter2 = 0xc00000f0,
            InvalidParameter3 = 0xc00000f1,
            InvalidParameter4 = 0xc00000f2,
            InvalidParameter5 = 0xc00000f3,
            InvalidParameter6 = 0xc00000f4,
            InvalidParameter7 = 0xc00000f5,
            InvalidParameter8 = 0xc00000f6,
            InvalidParameter9 = 0xc00000f7,
            InvalidParameter10 = 0xc00000f8,
            InvalidParameter11 = 0xc00000f9,
            InvalidParameter12 = 0xc00000fa,
            MappedFileSizeZero = 0xc000011e,
            TooManyOpenedFiles = 0xc000011f,
            Cancelled = 0xc0000120,
            CannotDelete = 0xc0000121,
            InvalidComputerName = 0xc0000122,
            FileDeleted = 0xc0000123,
            SpecialAccount = 0xc0000124,
            SpecialGroup = 0xc0000125,
            SpecialUser = 0xc0000126,
            MembersPrimaryGroup = 0xc0000127,
            FileClosed = 0xc0000128,
            TooManyThreads = 0xc0000129,
            ThreadNotInProcess = 0xc000012a,
            TokenAlreadyInUse = 0xc000012b,
            PagefileQuotaExceeded = 0xc000012c,
            CommitmentLimit = 0xc000012d,
            InvalidImageLeFormat = 0xc000012e,
            InvalidImageNotMz = 0xc000012f,
            InvalidImageProtect = 0xc0000130,
            InvalidImageWin16 = 0xc0000131,
            LogonServer = 0xc0000132,
            DifferenceAtDc = 0xc0000133,
            SynchronizationRequired = 0xc0000134,
            DllNotFound = 0xc0000135,
            IoPrivilegeFailed = 0xc0000137,
            OrdinalNotFound = 0xc0000138,
            EntryPointNotFound = 0xc0000139,
            ControlCExit = 0xc000013a,
            PortNotSet = 0xc0000353,
            DebuggerInactive = 0xc0000354,
            CallbackBypass = 0xc0000503,
            PortClosed = 0xc0000700,
            MessageLost = 0xc0000701,
            InvalidMessage = 0xc0000702,
            RequestCanceled = 0xc0000703,
            RecursiveDispatch = 0xc0000704,
            LpcReceiveBufferExpected = 0xc0000705,
            LpcInvalidConnectionUsage = 0xc0000706,
            LpcRequestsNotAllowed = 0xc0000707,
            ResourceInUse = 0xc0000708,
            ProcessIsProtected = 0xc0000712,
            VolumeDirty = 0xc0000806,
            FileCheckedOut = 0xc0000901,
            CheckOutRequired = 0xc0000902,
            BadFileType = 0xc0000903,
            FileTooLarge = 0xc0000904,
            FormsAuthRequired = 0xc0000905,
            VirusInfected = 0xc0000906,
            VirusDeleted = 0xc0000907,
            TransactionalConflict = 0xc0190001,
            InvalidTransaction = 0xc0190002,
            TransactionNotActive = 0xc0190003,
            TmInitializationFailed = 0xc0190004,
            RmNotActive = 0xc0190005,
            RmMetadataCorrupt = 0xc0190006,
            TransactionNotJoined = 0xc0190007,
            DirectoryNotRm = 0xc0190008,
            CouldNotResizeLog = 0xc0190009,
            TransactionsUnsupportedRemote = 0xc019000a,
            LogResizeInvalidSize = 0xc019000b,
            RemoteFileVersionMismatch = 0xc019000c,
            CrmProtocolAlreadyExists = 0xc019000f,
            TransactionPropagationFailed = 0xc0190010,
            CrmProtocolNotFound = 0xc0190011,
            TransactionSuperiorExists = 0xc0190012,
            TransactionRequestNotValid = 0xc0190013,
            TransactionNotRequested = 0xc0190014,
            TransactionAlreadyAborted = 0xc0190015,
            TransactionAlreadyCommitted = 0xc0190016,
            TransactionInvalidMarshallBuffer = 0xc0190017,
            CurrentTransactionNotValid = 0xc0190018,
            LogGrowthFailed = 0xc0190019,
            ObjectNoLongerExists = 0xc0190021,
            StreamMiniversionNotFound = 0xc0190022,
            StreamMiniversionNotValid = 0xc0190023,
            MiniversionInaccessibleFromSpecifiedTransaction = 0xc0190024,
            CantOpenMiniversionWithModifyIntent = 0xc0190025,
            CantCreateMoreStreamMiniversions = 0xc0190026,
            HandleNoLongerValid = 0xc0190028,
            NoTxfMetadata = 0xc0190029,
            LogCorruptionDetected = 0xc0190030,
            CantRecoverWithHandleOpen = 0xc0190031,
            RmDisconnected = 0xc0190032,
            EnlistmentNotSuperior = 0xc0190033,
            RecoveryNotNeeded = 0xc0190034,
            RmAlreadyStarted = 0xc0190035,
            FileIdentityNotPersistent = 0xc0190036,
            CantBreakTransactionalDependency = 0xc0190037,
            CantCrossRmBoundary = 0xc0190038,
            TxfDirNotEmpty = 0xc0190039,
            IndoubtTransactionsExist = 0xc019003a,
            TmVolatile = 0xc019003b,
            RollbackTimerExpired = 0xc019003c,
            TxfAttributeCorrupt = 0xc019003d,
            EfsNotAllowedInTransaction = 0xc019003e,
            TransactionalOpenNotAllowed = 0xc019003f,
            TransactedMappingUnsupportedRemote = 0xc0190040,
            TxfMetadataAlreadyPresent = 0xc0190041,
            TransactionScopeCallbacksNotSet = 0xc0190042,
            TransactionRequiredPromotion = 0xc0190043,
            CannotExecuteFileInTransaction = 0xc0190044,
            TransactionsNotFrozen = 0xc0190045,

            MaximumNtStatus = 0xffffffff
        }
        #endregion
    }
}

