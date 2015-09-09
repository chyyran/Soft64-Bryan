using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300.CP1;

namespace Soft64.MipsR4300
{
    public struct FPUEntity
    {
        private DataFormat m_Tag;
        private UInt64 m_Long = 0;
        private UInt32 m_Word = 0;
        private Single m_Single = 0.0F;
        private Double m_Double = 0.0d;
        private Boolean m_Wide;
        private FpuRegisters m_FPR;

        public FPUEntity(DataFormat format, ExecutionState state)
        {
            m_Tag = format;
            m_Wide = state.CP0Regs.StatusReg.AdditionalFPR;
            m_FPR = state.Fpr;
        }

        private static unsafe UInt32 _FIXED(Single single)
        {
            Single* ptr = (Single *)Marshal.AllocCoTaskMem(4);
            *ptr = single;
            UInt32 value = *(UInt32*)ptr;
            Marshal.FreeCoTaskMem((IntPtr)ptr);
            return value;
        }

        private static UInt32 _FIXED(UInt32 word)
        {
            return word;
        }

        private static unsafe UInt64 _FIXED(Double d)
        {
            Double* ptr = (Double*)Marshal.AllocCoTaskMem(8);
            *ptr = d;
            UInt64 value = *(UInt64*)ptr;
            Marshal.FreeCoTaskMem((IntPtr)ptr);
            return value;
        }

        private static UInt64 _FIXED(UInt64 dword)
        {
            return dword;
        }

        private unsafe static Single _FLOAT(UInt32 word)
        {
            UInt32* ptr = (UInt32*)Marshal.AllocCoTaskMem(4);
            *ptr = word;
            Single value = *(Single *)ptr;
            Marshal.FreeCoTaskMem((IntPtr)ptr);
            return value;
        }

        private unsafe static Double _FLOAT(UInt64 dword)
        {
            UInt64* ptr = (UInt64*)Marshal.AllocCoTaskMem(4);
            *ptr = dword;
            Double value = *(Double*)ptr;
            Marshal.FreeCoTaskMem((IntPtr)ptr);
            return value;
        }

        public void Load(Int32 index)
        {
            if (m_Tag == DataFormat.Reserved)
                throw new InvalidOperationException("Cannot operate on reserved data formats");

            if (m_Wide)
            {
                switch (m_Tag)
                {
                    case DataFormat.Word: m_Word = m_FPR.ReadFPR32Unsigned(index); break;
                    case DataFormat.Single: m_Single = m_FPR.ReadFPRSingle(index); break;
                    case DataFormat.Doubleword: m_Long = m_FPR.ReadFPRUnsigned(index); break;
                    case DataFormat.Double: m_Double = m_FPR.ReadFPRDouble(index); break;
                    default: break;
                }
            }
            else
            {
                switch (m_Tag)
                {
                    case DataFormat.Word: m_Word = m_FPR.ReadFPR32Unsigned(index); break;
                    case DataFormat.Single: m_Single = m_FPR.ReadFPRSingle(index); break;
                    case DataFormat.Doubleword:
                        {
                            m_Long = m_FPR.ReadFPR32Unsigned(index + 1);
                            m_Long <<= 32;
                            m_Long |= m_FPR.ReadFPR32Unsigned(index);
                            break;
                        }
                    case DataFormat.Double:
                        {
                            UInt64 l = m_FPR.ReadFPR32Unsigned(index + 1);
                            l <<= 32;
                            l |= m_FPR.ReadFPR32Unsigned(index);
                            m_Double = _FLOAT(l);
                            break;
                        }
                    default: break;
                }
            }
        }

        public void Store(Int32 index)
        {
            if (m_Tag == DataFormat.Reserved)
                throw new InvalidOperationException("Cannot operate on reserved data formats");

            if (m_Wide)
            {
                if (m_Tag == DataFormat.Word || m_Tag == DataFormat.Single)
                {
                    m_FPR.WriteFPR32Unsigned(index, _FIXED(Value));
                }
                
                if (m_Tag == DataFormat.Double || m_Tag == DataFormat.Doubleword)
                {
                    m_FPR.WriteFPRUnsigned(index, _FIXED(Value));
                }
            }
            else
            {
                if (m_Tag == DataFormat.Word || m_Tag == DataFormat.Single)
                {
                    m_FPR.WriteFPR32Unsigned(index + 1, 0);
                    m_FPR.WriteFPR32Unsigned(index, _FIXED(Value));
                }

                if (m_Tag == DataFormat.Double || m_Tag == DataFormat.Doubleword)
                {
                    UInt64 value = _FIXED(Value);
                    m_FPR.WriteFPRUnsigned(index + 1, value >> 32);
                    m_FPR.WriteFPRUnsigned(index, value & 0xFFFFFFFF);
                }
            }
        }

        public void RunFPUOp(Func<dynamic, dynamic> opFunction)
        {
            /* TODO: */
        }

        public dynamic Value
        {
            get
            {
                switch (m_Tag)
                {
                    case DataFormat.Double: return m_Double;
                    case DataFormat.Doubleword: return m_Long;
                    case DataFormat.Single: return m_Single;
                    case DataFormat.Word: return m_Word;
                    default:
                    case DataFormat.Reserved: throw new InvalidOperationException("Cannot return a value with reserved data format");
                }
            }
        }
    }
}
