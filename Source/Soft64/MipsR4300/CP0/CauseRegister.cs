using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.CP0
{
    [RegisterField("ExcCode", 5, 2, typeof(Byte))]
    [RegisterField("IP0", 1, 8, typeof(Boolean))]
    [RegisterField("IP1", 1, 9, typeof(Boolean))]
    [RegisterField("IP2", 1, 10, typeof(Boolean))]
    [RegisterField("IP3", 1, 11, typeof(Boolean))]
    [RegisterField("IP4", 1, 12, typeof(Boolean))]
    [RegisterField("IP5", 1, 13, typeof(Boolean))]
    [RegisterField("IP6", 1, 14, typeof(Boolean))]
    [RegisterField("IP7", 1, 15, typeof(Boolean))]
    [RegisterField("CE", 2, 28, typeof(Byte))]
    [RegisterField("BD", 2, 30, typeof(Byte))]
    public sealed class CauseRegister : SmartRegister<UInt32>
    {
        public CauseRegister() : base()
        {

        }

        public UInt64 RegisterValue64
        {
            get { return (UInt64)RegisterValue; }
            set { RegisterValue = (UInt32)value; }
        }

        public Boolean IP0
        {
            get { return AutoRegisterProps.GetIP0(); }
            set { AutoRegisterProps.SetIP0(value); }
        }

        public Boolean IP1
        {
            get { return AutoRegisterProps.GetIP1(); }
            set { AutoRegisterProps.SetIP1(value); }
        }

        public Boolean IP2
        {
            get { return AutoRegisterProps.GetIP2(); }
            set { AutoRegisterProps.SetIP2(value); }
        }

        public Boolean IP3
        {
            get { return AutoRegisterProps.GetIP3(); }
            set { AutoRegisterProps.SetIP3(value); }
        }

        public Boolean IP4
        {
            get { return AutoRegisterProps.GetIP4(); }
            set { AutoRegisterProps.SetIP4(value); }
        }

        public Boolean IP5
        {
            get { return AutoRegisterProps.GetIP5(); }
            set { AutoRegisterProps.SetIP5(value); }
        }

        public Boolean IP6
        {
            get { return AutoRegisterProps.GetIP6(); }
            set { AutoRegisterProps.SetIP6(value); }
        }

        public Boolean IP7
        {
            get { return AutoRegisterProps.GetIP7(); }
            set { AutoRegisterProps.SetIP7(value); }
        }
        public Byte CE
        {
            get { return AutoRegisterProps.GetCE(); }
            set { AutoRegisterProps.SetCE(value); }
        }
        public Byte BD
        {
            get { return AutoRegisterProps.BD(); }
            set { AutoRegisterProps.BD(value); }
        }

        public Byte ExcCode
        {
            get { return AutoRegisterProps.GetExcCode(); }
            set { AutoRegisterProps.SetExcCode(value); }
        }

        public ExceptionCode ExceptionType
        {
            get { return TranslateFromReg(ExcCode); }
            set { ExcCode = TranslateFromCode(value); }
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
