using System;
using static KernelSharp.WDK;
using static KernelSharp.WDK.Undocumented;

namespace KernelBypassSharp
{
    public static unsafe class Util
    {
        public static PVOID GetKernelModuleByName(string moduleName)
        {
            uint poolSize = 0;
            
            //get estimated size first
            NTSTATUS status = ZwQuerySystemInformation(SystemInformationClass.SystemModuleInformation, null, 0, &poolSize);

            if (status != NTSTATUS.InfoLengthMismatch)
                return null;

            var sysModInfo = (RTL_PROCESS_MODULES*)ExAllocatePool(PoolType.NonPagedPool, poolSize);

            if (sysModInfo == null) return null;

            status = ZwQuerySystemInformation(SystemInformationClass.SystemModuleInformation, sysModInfo, poolSize, null);

            PVOID address = 0;
            char* s_moduleName = moduleName.c_str();
            RTL_PROCESS_MODULE_INFORMATION* moduleInfo = (RTL_PROCESS_MODULE_INFORMATION*)((ulong)sysModInfo + 8);

            for (uint i = 0; i < sysModInfo->NumberOfModules; i++)
            {
                var moduleEntry = moduleInfo[i];

                if(strstr((char*)moduleEntry.FullPathName, s_moduleName) != null)
                {
                    address = moduleEntry.ImageBase;
                }
            }

            ExFreePool(sysModInfo);
            ExFreePool(s_moduleName);

            return address;
        }
    }
}
