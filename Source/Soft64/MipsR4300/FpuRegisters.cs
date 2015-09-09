using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.CP1
{
    public sealed class FpuRegisters : IDisposable
    {
        private GCHandle m_RegHandle;
        private UInt64[] m_Regs;
        private IntPtr m_RegsPtr;

        public FpuRegisters()
        {
            m_Regs = new ulong[32];
            m_RegHandle = GCHandle.Alloc(m_Regs, GCHandleType.Pinned);
            /* TODO: Free handle on dispose */
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
                UInt64* ptr = (UInt64*)m_RegsPtr;
                return *(Double*)(ptr + index);
            }
        }

        public void WriteFPRDouble(Int32 index, Double value)
        {
            unsafe
            {
                UInt64* ptr = (UInt64*)m_RegsPtr;
                *(Double*)(ptr + index) = value;
            }
        }

        public Single ReadFPRSingle(Int32 index)
        {
            unsafe
            {
                UInt64* ptr = (UInt64*)m_RegsPtr;
                return *(Single*)(ptr + index);
            }
        }

        public void WriteFPRSingle(Int32 index, Single value)
        {
            unsafe
            {
                UInt64* ptr = (UInt64*)m_RegsPtr;
                *(Single*)(ptr + index) = value;
            }
        }

        public UInt64 Condition { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
