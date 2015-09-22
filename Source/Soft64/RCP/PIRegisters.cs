using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.RCP
{
    public sealed class PIRegisters : MmioStream
    {
        private const Int32 OFFSET_PI_DRAM_ADDR_REG = 0;
        private const Int32 OFFSET_PI_CART_ADDR_REG = 4;
        private const Int32 OFFSET_PI_RD_LEN_REG = 8;
        private const Int32 OFFSET_PI_WR_LEN_REG = 0xC;
        private const Int32 OFFSET_PI_STATUS_REG = 0x10;
        private const Int32 OFFSET_PI_BSD_DOM1_LAT_REG = 0x14;
        private const Int32 OFFSET_PI_BSD_DOM1_PWD_REG = 0x18;
        private const Int32 OFFSET_PI_BSD_DOM1_PGS_REG = 0x1C;
        private const Int32 OFFSET_PI_BSD_DOM1_RLS_REG = 0x20;
        private const Int32 OFFSET_PI_BSD_DOM2_LAT_REG = 0x24;
        private const Int32 OFFSET_PI_BSD_DOM2_PWD_REG = 0x28;
        private const Int32 OFFSET_PI_BSD_DOM2_PGS_REG = 0x2C;
        private const Int32 OFFSET_PI_BSD_DOM2_RLS_REG = 0x30;

        public PIRegisters() : base(0xFFFFF)
        {

        }

        public UInt32 DramAddress
        {
            get { return ReadUInt32(OFFSET_PI_DRAM_ADDR_REG); }
            set { Write(OFFSET_PI_DRAM_ADDR_REG, value); }
        }

        public UInt32 CartridgeAddress
        {
            get { return ReadUInt32(OFFSET_PI_CART_ADDR_REG); }
            set { Write(OFFSET_PI_CART_ADDR_REG, value); }
        }

        public UInt32 ReadLength
        {
            get { return ReadUInt32(OFFSET_PI_RD_LEN_REG); }
            set { Write(OFFSET_PI_RD_LEN_REG, value); }
        }

        public UInt32 WriteLength
        {
            get { return ReadUInt32(OFFSET_PI_WR_LEN_REG); }
            set { Write(OFFSET_PI_WR_LEN_REG, value); }
        }

        public UInt32 Status
        {
            get { return ReadUInt32(OFFSET_PI_STATUS_REG); }
            set { Write(OFFSET_PI_STATUS_REG, value); }
        }

        public UInt32 Domain1Latency
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM1_LAT_REG); }
            set { Write(OFFSET_PI_BSD_DOM1_LAT_REG, value); }
        }

        public UInt32 Domain1PulseWidth
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM1_PWD_REG); }
            set { Write(OFFSET_PI_BSD_DOM1_PWD_REG, value); }
        }

        public UInt32 Domain1PageSize
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM1_PGS_REG); }
            set { Write(OFFSET_PI_BSD_DOM1_PGS_REG, value); }
        }

        public UInt32 Domain1Release
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM1_RLS_REG); }
            set { Write(OFFSET_PI_BSD_DOM1_RLS_REG, value); }
        }

        public UInt32 Domain2Latency
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM2_LAT_REG); }
            set { Write(OFFSET_PI_BSD_DOM2_LAT_REG, value); }
        }

        public UInt32 Domain2PulseWidth
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM2_PWD_REG); }
            set { Write(OFFSET_PI_BSD_DOM2_PWD_REG, value); }
        }

        public UInt32 Domain2PageSize
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM2_PGS_REG); }
            set { Write(OFFSET_PI_BSD_DOM2_PGS_REG, value); }
        }

        public UInt32 Domain2Release
        {
            get { return ReadUInt32(OFFSET_PI_BSD_DOM2_RLS_REG); }
            set { Write(OFFSET_PI_BSD_DOM2_RLS_REG, value); }
        }
    }
}
