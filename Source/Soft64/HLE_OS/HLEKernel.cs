/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using Soft64.MipsR4300;
using Soft64.MipsR4300.CP0;

namespace Soft64.HLE_OS
{
    public sealed class HLEKernel
    {
        private Dictionary<UInt64, Int32> m_TLBEntries;
        private Boolean m_SetGlobalPageSize = false;
        private PageSize m_GlobalPageSize;

        public HLEKernel()
        {
            m_TLBEntries = new Dictionary<UInt64, int>();
        }

        public void CreateProcess(ELFExecutable executable)
        {
            LoadSegments(executable);

            ExecutionState state = Machine.Current.CPU.State;

            /* Setup 64-bit kernel mode */
            state.CP0Regs.HLStatusRegister.KSUMode = RingMode.Kernel;
            state.CP0Regs.HLStatusRegister.KernelX = true;
            state.GPRRegs64[29] = 0x00000000A4001000; // Setup Stack Pointer to point in IMEM

            Machine.Current.CPU.State.PC = (Int64)executable.EntryPointAddress;
        }

        private void LoadSegments(ELFExecutable executable)
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

                if (!m_SetGlobalPageSize)
                {
                    m_GlobalPageSize = new PageSize(entry.Align);
                    m_SetGlobalPageSize = true;
                }

                var tlbEntry = new TLBEntry(WordSize.MIPS32, 0, entry.VAddress, new PageSize(entry.Align));

                /* Convert the physical address into a PFN */
                Int32 pfnOddCalc = (Int32)(pAddress / tlbEntry.Size.Size);

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

                /* Update our local index list */
                m_TLBEntries.Add(tlbEntry.VPN2.Vpn2, tlbIndex);

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

        public void PrepareTLB(Int64 vaddress)
        {
            Int32 index = 0;

            var tempEntry = new TLBEntry(WordSize.MIPS32, 0, vaddress, m_GlobalPageSize);

            if (m_TLBEntries.TryGetValue(tempEntry.VPN2.Vpn2, out index))
            {
                Machine.Current.CPU.VirtualMemoryStream.TLB.Index = (UInt64)index;
                Machine.Current.CPU.VirtualMemoryStream.TLB.Read();
            }
        }
    }
}