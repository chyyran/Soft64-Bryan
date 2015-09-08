using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Load(Int32 index)
        {
            if (m_Tag == DataFormat.Reserved)
                throw new InvalidOperationException("Cannot operate on reserved data formats");

            m_Long = m_FPR.ReadFPRUnsigned(index);
            m_Word = m_FPR.ReadFPR32Unsigned(index);

            if (m_Wide)
            {
                switch (m_Tag)
                {
                    case DataFormat.Word: break;

                    case DataFormat.Single:
                        {
                            unsafe
                            {
                                fixed (UInt32* ptr = &m_Word)
                                {
                                    m_Single = *(Single*)ptr;
                                }

                                break;
                            }
                        }

                    case DataFormat.Doubleword: break;

                    case DataFormat.Double:
                        {
                            unsafe
                            {
                                fixed (UInt64* ptr = &m_Long)
                                {
                                    m_Double = *(Double*)ptr;
                                }
                            }

                            break;
                        }
                }
            }
            else
            {
                m_Long =
                    (m_FPR.ReadFPR32Unsigned(index) << 32) |
                     m_FPR.ReadFPR32Unsigned(index + 1);

                switch (m_Tag)
                {
                    case DataFormat.Word: break;

                    case DataFormat.Single:
                        {
                            unsafe
                            {
                                fixed (UInt32* ptr = &m_Word)
                                {
                                    m_Single = *(Single*)ptr;
                                }

                                break;
                            }
                        }

                    case DataFormat.Doubleword: break;

                    case DataFormat.Double:
                        {
                            unsafe
                            {
                                fixed (UInt64* ptr = &m_Long)
                                {
                                    m_Double = *(Double*)ptr;
                                }
                            }

                            break;
                        }
                }
            }
        }

        public void Store(Int32 index)
        {
            // TODO
            //if (m_Wide)
            //{
            //    switch (m_Tag)
            //    {
            //        case DataFormat.Single:
            //    }


            //    if (m_Tag == DataFormat.Single || m_Tag == DataFormat.Word)
            //    {
            //        m_FPR.WriteFPR32Unsigned(index, (UInt32)Value);
            //    }

            //    if (m_Tag == DataFormat.Double || m_Tag == DataFormat.Doubleword)
            //    {
            //        m_FPR.WriteFPRUnsigned(index, value);
            //    }
            //}
            //else
            //{
            //    m_FPR.WriteFPR32Unsigned(index, (UInt32)value);

            //    if (m_Tag == DataFormat.Single || m_Tag == DataFormat.Word)
            //    {
            //        m_FPR.WriteFPR32Unsigned(index + 1, 0);
            //    }

            //    if (m_Tag == DataFormat.Double || m_Tag == DataFormat.Doubleword)
            //    {
            //        m_FPR.WriteFPR32Unsigned(index + 1, (UInt32)(value >> 32));
            //    }
            //}
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
