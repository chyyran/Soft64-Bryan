using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.CP0
{
    public sealed class CauseRegister
    {
        private UInt32 m_Reg;

        public UInt32 RegValue
        {
            get { return m_Reg; }
            set { m_Reg = value; }
        }

        public ExceptionCode ExcCode
        {
            get
            {
                Byte code = (Byte)((0xFFFFFF80 & m_Reg) >> 2);
                return TranslateFromReg(code);
            }

            set
            {

            }
        }


        private ExceptionCode TranslateFromReg(byte value)
        {
            if ((value >= 0 && value <= 15) || value == 23 || value == 31)
            {
                return (ExceptionCode)value;
            }
            else
            {
                return ExceptionCode.Invalid;
            }
        }

        private Byte TranslateFromCode(ExceptionCode code)
        {
            if (code == ExceptionCode.Invalid)
                throw new ArgumentException("code");

            return (Byte)code;
        }
    }

    public enum ExceptionCode : byte
    {
        Interrupt = 0,
        TlbMod,
        TlbLoad,
        TlbStore,
        AddressErrorLoad,
        AddressErrorStore,
        BusErrorInstruction,
        BusErrorData,
        Syscall,
        Breakpoint,
        ReservedInstruction,
        CopUnstable,
        OverFlow,
        Trap,
        VCEInstruction,
        FloatingPoint,
        Watch = 23,
        VCEData = 31,
        Invalid
    }
}
