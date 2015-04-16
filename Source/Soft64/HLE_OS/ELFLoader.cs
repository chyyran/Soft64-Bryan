using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300;
using Soft64.MipsR4300.CP0;

namespace Soft64.HLE_OS
{
    public static class ELFLoader
    {
        private static List<Int32> m_Indicies = new List<int>();

        public static void LoadElf(ELFExecutable executable)
        {
            /* Very simple loader for elf */
            CPUProcessor cpu = Machine.Current.CPU;

            /* Setup the CPU register state */
            /* Setup 64-bit kernel mode */
            cpu.State.CP0Regs.HLStatusRegister.KSUMode = RingMode.Kernel;
            cpu.State.CP0Regs.HLStatusRegister.KernelX = true;
            cpu.State.GPRRegs64[29] = 0x00000000A4001000; // Setup Stack Pointer to point in IMEM

            /* Set the entry point */
            Machine.Current.CPU.State.PC = (Int64)executable.EntryPointAddress;

            /* Load sections into memory and setup TLB entries */
            LoadSegments(executable);

            /* Set the TLB index to the first section that will be used */
            
            /* Prepare first entry */

        }

        private static void LoadSegments(ELFExecutable executable)
        {
            var entries = executable.ProgramHeaderEntries;

            for (Int32 i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                /* Skip- non loadable segments */
                if (entry.Type != 1)
                    continue;

                /* Write the segment into virtual memory */
                Boolean nextEntryValid = i + 1 < entries.Length;
                ELFProgramHeaderEntry nextEntry = default(ELFProgramHeaderEntry);

                if (nextEntryValid)
                {
                    nextEntry = entries[i + 1];
                }

                executable.CopyProgramSegment(
                    entry, 
                    nextEntryValid, 
                    nextEntry, 
                    Machine.Current.CPU.VirtualMemoryStream, 
                    0xA0000000);
            }
        }
    }
}
