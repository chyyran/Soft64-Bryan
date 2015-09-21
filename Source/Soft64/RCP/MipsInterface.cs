using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.RCP
{
    enum MiInterrupt
    {
        Sp = 0x01,
        Si = 0x02,
        Ai = 0x04,
        Vi = 0x08,
        Pi = 0x10,
        Dp = 0x20
    };

    public sealed class MipsInterface : MmioStream
    {
        private const Int32 OFFSET_MI_MODE_REG = 0;
        private const Int32 OFFSET_MI_VERSION_REG = 4;
        private const Int32 OFFSET_MI_INTR_REG = 8;
        private const Int32 OFFSET_MI_INTR_MASK_REG = 0xC;

        public MipsInterface() : base(0xFFFFF)
        {
            
        }

        public void RaiseInterrupt(UInt32 interrupt)
        {
            Interrupts |= interrupt;

            if ((Interrupts & InterruptMask) != 0)
            {
                Machine.Current.DeviceCPU.RaiseMaskableInterrupt(0x400);
            }
        }

        public UInt32 Mode
        {
            get { return ReadUInt32(OFFSET_MI_MODE_REG); }
            set { Write(OFFSET_MI_MODE_REG, value); }
        }

        public UInt32 Version
        {
            get { return ReadUInt32(OFFSET_MI_VERSION_REG); }
            set { Write(OFFSET_MI_VERSION_REG, value); }
        }

        public UInt32 Interrupts
        {
            get { return ReadUInt32(OFFSET_MI_INTR_REG); }
            set { Write(OFFSET_MI_INTR_REG, value); }
        }

        public UInt32 InterruptMask
        {
            get { return ReadUInt32(OFFSET_MI_INTR_MASK_REG); }
            set { Write(OFFSET_MI_INTR_MASK_REG, value); }
        }
    }
}
