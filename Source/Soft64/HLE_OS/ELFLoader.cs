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
            /* Since creating only one process per emulation, just start with the virtual pages in RDRAM */
            Int64 pAddress = 0x0;

            /* Make a list of virtual pages to setup into the TLB */
            var entries = executable.ProgramHeaderEntries;

            /* For now will only setup 1 TLB entry per section */
            Int32 tlbIndex = 0;

            var tlb = Machine.Current.CPU.VirtualMemoryStream.TLB;

            for (Int32 i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                /* Skip- non loadable segments */
                if (entry.Type != 1)
                    continue;

                var tlbEntry = new TLBEntry(WordSize.MIPS32, 0, entry.VAddress, new PageSize(entry.Align));

                /* Convert the physical address into a PFN */
                Int32 pfnOddCalc = (Int32)(pAddress / tlbEntry.Size.Size);

                /* TODO: Does odd/even matter? */

                PageFrameNumber pfnOdd = new PageFrameNumber();
                pfnOdd.CoherencyBits = 0;
                pfnOdd.IsDirty = false;
                pfnOdd.IsGlobal = false;
                pfnOdd.IsValid = true;
                pfnOdd.PFN = pfnOddCalc;

                PageFrameNumber pfnEven = new PageFrameNumber();
                pfnEven.CoherencyBits = 0;
                pfnEven.IsDirty = false;
                pfnEven.IsGlobal = false;
                pfnEven.IsValid = false;
                pfnEven.PFN = 0;

                tlbEntry.PfnOdd = pfnOdd;
                tlbEntry.PfnEven = pfnEven;

                tlb.AddEntry(tlbIndex, tlbEntry);

                /* Now force the TLB registers to be filled ready for TLB based memory operations */
                tlb.Index = (UInt64)tlbIndex;
                tlb.Read();

                /* Write the segment into virtual memory */
                Boolean nextEntryValid = i + 1 < entries.Length;
                ELFProgramHeaderEntry nextEntry = default(ELFProgramHeaderEntry);

                if (nextEntryValid)
                {
                    nextEntry = entries[i + 1];
                }

                executable.CopySegment(entry, nextEntryValid, nextEntry, Machine.Current.CPU.VirtualMemoryStream);

                pAddress += tlbEntry.Size.Size;
                tlbIndex++;
            }
        }

        private static void PrepareTLB(Int32 index)
        {
            Machine.Current.CPU.VirtualMemoryStream.TLB.Index = index;
            Machine.Current.CPU.VirtualMemoryStream.TLB.Read();
        }
    }
}
