using System;
using static KernelSharp.NTImage;
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


        //based on https://github.com/btbd/hwid/blob/master/Kernel/util.c
        private static bool CheckMask(byte* addr, byte* pattern, byte* mask)
        {
            while(*mask != 0)
            {
                if ('x' == *mask && *addr != *pattern)
                    return false;

                mask += 2;  //C# strings are 2 bytes long, so we skip 2 bytes here
                pattern += 2;
                ++addr;
            }
            return true;
        }

        //based on https://github.com/btbd/hwid/blob/master/Kernel/util.c
        public static PVOID FindPattern(byte* address, uint length, byte* pattern, byte* mask)
        {
            length -= (uint)wcslen((char*)mask);
            for(uint i = 0; i <= length; ++i)
            {
                byte* addr = &address[i];
                if (CheckMask(addr, pattern, mask))
                    return addr;
            }

            return null;
        }

        //based on https://github.com/btbd/hwid/blob/master/Kernel/util.c
        public static PVOID FindPatternImage(byte* address, string pattern, string mask)
        {
            PVOID match = 0;

            IMAGE_NT_HEADERS64* headers = (IMAGE_NT_HEADERS64*)(address + ((IMAGE_DOS_HEADER*)address)->e_lfanew);
            IMAGE_SECTION_HEADER* sections = IMAGE_FIRST_SECTION(headers);

            var textSectName = ".text".c_str();
            var s_pattern = pattern.w_str();
            var s_mask = mask.w_str();

            for (uint i = 0; i < headers->FileHeader.NumberOfSections; ++i)
            {
                IMAGE_SECTION_HEADER* section = &sections[i];
                
                    //PAGE
                if (0x45474150 == *(int*)section->Name || memcmp(section->Name, textSectName, 5) == 0)
                {
                    match = FindPattern(address + section->VirtualAddress, section->VirtualSize, (byte*)s_pattern, (byte*)s_mask);
                    if ((ulong)match != 0) break;
                }
            }

            ExFreePool(textSectName);

            return match;
        }
    }
}
