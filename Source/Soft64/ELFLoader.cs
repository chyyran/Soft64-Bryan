using System;
using Soft64.MipsR4300;

namespace Soft64
{
    /// <summary>
    /// Loads elf into RDRAM and puts stack in IMEM.
    /// </summary>
    public static class ELFLoader
    {
        public static void LoadElf(ELFExecutable executable)
        {
            /* Get the current processor instance */
            CPUProcessor cpu = Machine.Current.DeviceCPU;

            /* Setup the CPU for the executable */
            cpu.State.CP0Regs.StatusReg.RingFlags = RingMode.Kernel; /* Running whole program in lowest ring mode */
            cpu.State.CP0Regs.StatusReg.KernelAddressingMode = true;            /* We want kernel 64 bit addressing */
            cpu.State.GPRRegs64[29] = 0x00000000A4001000;                 /* Create a stack pointer pointing into IMEM */

            /* Set the entry point */
            Machine.Current.DeviceCPU.State.PC = (Int64)executable.EntryPointAddress;

            /* Load sections into memory and setup TLB entries */
            LoadSegments(executable);
        }

        private static void LoadSegments(ELFExecutable executable)
        {
            var progEntries = executable.ProgramHeaderEntries;

            for (Int32 i = 0; i < progEntries.Length; i++)
            {
                var entry = progEntries[i];

                /* Skip non loadable segments */
                if (entry.Type != 1)
                    continue;

                /* Write the segment into virtual memory */
                Boolean nextEntryValid = i + 1 < progEntries.Length;
                ELFProgramHeaderEntry nextEntry = default(ELFProgramHeaderEntry);

                if (nextEntryValid)
                {
                    nextEntry = progEntries[i + 1];
                }

                executable.CopyProgramSegment(
                    entry,
                    nextEntryValid,
                    nextEntry,
                    Machine.Current.DeviceCPU.VirtualMemoryStream,
                    0xA0000000);
            }
        }
    }
}