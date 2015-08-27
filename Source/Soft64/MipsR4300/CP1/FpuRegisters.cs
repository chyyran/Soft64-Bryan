using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.CP1
{
    public sealed class FpuRegisters
    {
        private UInt64[] m_Regs;

        public FpuRegisters()
        {
            m_Regs = new ulong[32];
        }

        public void Clear()
        {
            Array.Clear(m_Regs, 0, m_Regs.Length);
        }

        public UInt64 ReadFPRUnsigned(Int32 index)
        {
            return (UInt64)m_Regs[index];
        }

        public void WriteFPRUnsigned(Int32 index, UInt64 value)
        {
            m_Regs[index] = value;
        }

        public UInt32 ReadFPR32Unsigned(Int32 index)
        {
            return (UInt32)m_Regs[index];
        }

        public void WriteFPR32Unsigned(Int32 index, UInt32 value)
        {
            m_Regs[index] = value;
        }

        public Double ReadFPRDouble(Int32 index)
        {
            unsafe
            {
                fixed (UInt64 * ptr = m_Regs)
                {
                    return *(Double*)(UInt64*)(ptr + index);
                }
            }
        }

        public void WriteFPRDouble(Int32 index, Double value)
        {
            unsafe
            {
                fixed (UInt64* ptr = m_Regs)
                {
                    *(Double*)(UInt64*)(ptr + index) = value;
                }
            }
        }

        public Single ReadFPRSingle(Int32 index)
        {
            unsafe
            {
                fixed (UInt64* ptr = m_Regs)
                {
                    return *(Single*)(UInt32*)(ptr + index);
                }
            }
        }

        public void WriteFPRSingle(Int32 index, Single value)
        {
            unsafe
            {
                fixed (UInt64* ptr = m_Regs)
                {
                    *(Single*)(UInt32*)(ptr + index) = value;
                }
            }
        }
    }
}
