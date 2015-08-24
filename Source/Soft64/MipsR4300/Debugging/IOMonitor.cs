using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    public delegate void MemoryExecute(N64MemRegions region, Int64 address);

    public sealed class IOMonitor
    {
        public event MemoryExecute CPUMemoryRead;
        public event MemoryExecute CPUMemoryWrite;

        internal IOMonitor()
        {

        }

        internal void CPUMemRead(Int64 address)
        {
            N64MemRegions region = GetRegion(address);

            var e = CPUMemoryRead;

            if (e != null)
            {
                e(region, address);
            }
        }

        internal void CPUMemWrite(Int64 address)
        {
            N64MemRegions region = N64MemRegions.Unused;

            region = GetRegion(address);

            var e = CPUMemoryWrite;

            if (e != null)
            {
                e(region, address);
            }
        }

        private static N64MemRegions GetRegion(Int64 _address)
        {
            N64MemRegions region = N64MemRegions.Unused;
            UInt64 address = (UInt64)_address;

            if (address >= 0x00000000 && address <= 0x7FFFFFFF ||
                address >= 0xC0000000 && address <= 0xDFFFFFFF ||
                address >= 0xE0000000 && address <= 0xFFFFFFFF)
            {
                region = N64MemRegions.TLBMapped;
            }
            else
            {
                UInt64 poffset = address & 0x1FFFFFFF;

                if (poffset >= 0x00000000 && poffset < 0x03F00000 - 1)
                    region = N64MemRegions.Rdram;
                else if (poffset >= 0x03F00000 && poffset < 0x04000000 - 1)
                    region = N64MemRegions.RdramRegs;
                else if (poffset >= 0x04000000 && poffset < 0x04001000 - 1)
                    region = N64MemRegions.SPDMem;
                else if (poffset >= 0x04001000 && poffset < 0x04040000 - 1)
                    region = N64MemRegions.SPIMem;
                else if (poffset >= 0x04040000 && poffset < 0x04100000 - 1)
                    region = N64MemRegions.SPRegs;
                else if (poffset >= 0x04100000 && poffset < 0x04200000 - 1)
                    region = N64MemRegions.DPCmdRegs;
                else if (poffset >= 0x04200000 && poffset < 0x04300000 - 1)
                    region = N64MemRegions.DPSpanRegs;
                else if (poffset >= 0x04300000 && poffset < 0x04400000 - 1)
                    region = N64MemRegions.MIRegs;
                else if (poffset >= 0x04400000 && poffset < 0x04500000 - 1)
                    region = N64MemRegions.VIRegs;
                else if (poffset >= 0x04500000 && poffset < 0x04600000 - 1)
                    region = N64MemRegions.AIRegs;
                else if (poffset >= 0x04600000 && poffset < 0x04700000 - 1)
                    region = N64MemRegions.PIRegs;
                else if (poffset >= 0x04700000 && poffset < 0x04800000 - 1)
                    region = N64MemRegions.RIRegs;
                else if (poffset >= 0x04800000 && poffset < 0x04900000 - 1)
                    region = N64MemRegions.SIRegs;
                else if (poffset >= 0x04900000 && poffset < 0x05000000 - 1)
                    region = N64MemRegions.Unused;
                else if (poffset >= 0x05000000 && poffset < 0x06000000 - 1)
                    region = N64MemRegions.CD2A1;
                else if (poffset >= 0x06000000 && poffset < 0x08000000 - 1)
                    region = N64MemRegions.CD1A1;
                else if (poffset >= 0x08000000 && poffset < 0x10000000 - 1)
                    region = N64MemRegions.CD2A2;
                else if (poffset >= 0x10000000 && poffset < 0x1FC00000 - 1)
                    region = N64MemRegions.CD1A2;
                else if (poffset >= 0x1FC00000 && poffset < 0x1FD00000 - 1)
                    region = N64MemRegions.PIFMem;
                else if (poffset >= 0x1FD00000 && poffset < 0x80000000 - 1)
                    region = N64MemRegions.CD1A3;
                else
                    region = N64MemRegions.SysAD;

            }
            return region;
        }

        public Boolean Enabled { get; set; }
    }
}
