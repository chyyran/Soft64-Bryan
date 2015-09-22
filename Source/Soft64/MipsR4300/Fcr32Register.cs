using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    [RegisterField("RM", 2, 0, typeof(Int32))]
    [RegisterField("FLAG_I", 1, 2, typeof(Boolean))]
    [RegisterField("FLAG_U", 1, 3, typeof(Boolean))]
    [RegisterField("FLAG_O", 1, 4, typeof(Boolean))]
    [RegisterField("FLAG_Z", 1, 5, typeof(Boolean))]
    [RegisterField("FLAG_V", 1, 6, typeof(Boolean))]
    [RegisterField("ENABLE_I", 1, 7, typeof(Boolean))]
    [RegisterField("ENABLE_U", 1, 8, typeof(Boolean))]
    [RegisterField("ENABLE_O", 1, 9, typeof(Boolean))]
    [RegisterField("ENABLE_Z", 1, 10, typeof(Boolean))]
    [RegisterField("ENABLE_V", 1, 11, typeof(Boolean))]
    [RegisterField("CAUSE_I", 1, 12, typeof(Boolean))]
    [RegisterField("CAUSE_U", 1, 13, typeof(Boolean))]
    [RegisterField("CAUSE_O", 1, 14, typeof(Boolean))]
    [RegisterField("CAUSE_Z", 1, 15, typeof(Boolean))]
    [RegisterField("CAUSE_V", 1, 16, typeof(Boolean))]
    [RegisterField("CAUSE_E", 1, 17, typeof(Boolean))]
    [RegisterField("C", 1, 23, typeof(Boolean))]
    [RegisterField("FS", 1, 24, typeof(Boolean))]
    public sealed class Fcr32Register : SmartRegister<UInt32>
    {
        public Fcr32Register() : base()
        {

        }

        public FPURoundMode RM
        {
            get
            {
                Int32 rm = AutoRegisterProps.GetRM();
                switch (rm)
                {
                    default:
                    case 0: return FPURoundMode.Near;
                    case 1: return FPURoundMode.Chop;
                    case 2: return FPURoundMode.Up;
                    case 3: return FPURoundMode.Down;
                }
            }

            set
            {
                switch (value)
                {
                    default:
                    case FPURoundMode.Near: AutoRegisterProps.SetRM(0); break;
                    case FPURoundMode.Chop: AutoRegisterProps.SetRM(1); break;
                    case FPURoundMode.Up: AutoRegisterProps.SetRM(2); break;
                    case FPURoundMode.Down: AutoRegisterProps.SetRM(3); break;
                }
            }
        }

        public Boolean FlagInexact
        {
            get { return AutoRegisterProps.GetFLAG_I(); }
            set { AutoRegisterProps.SetFLAG_I(value); }
        }

        public Boolean FlagUnderflow
        {
            get { return AutoRegisterProps.GetFLAG_U(); }
            set { AutoRegisterProps.SetFLAG_U(value); }
        }

        public Boolean FlagOverflow
        {
            get { return AutoRegisterProps.GetFLAG_O(); }
            set { AutoRegisterProps.SetFLAG_O(value); }
        }

        public Boolean FlagDivideByZero
        {
            get { return AutoRegisterProps.GetFLAG_Z(); }
            set { AutoRegisterProps.SetFLAG_Z(value); }
        }

        public Boolean FlagInvalidOperation
        {
            get { return AutoRegisterProps.GetFLAG_V(); }
            set { AutoRegisterProps.SetFLAG_V(value); }
        }

        public Boolean EnableInexact
        {
            get { return AutoRegisterProps.GetENABLE_I; }
            set { AutoRegisterProps.SetENABLE_I(value); }
        }

        public Boolean EnableUnderflow
        {
            get { return AutoRegisterProps.GetENABLE_U(); }
            set { AutoRegisterProps.SetENABLE_U(value); }
        }

        public Boolean EnableOverflow
        {
            get { return AutoRegisterProps.GetENABLE_O(); }
            set { AutoRegisterProps.SetENABLE_O(value); }
        }

        public Boolean EnableDivideByZero
        {
            get { return AutoRegisterProps.GetENABLE_Z(); }
            set { AutoRegisterProps.SetENABLE_Z(value); }
        }

        public Boolean EnableInvalidOperation
        {
            get { return AutoRegisterProps.GetENABLE_V(); }
            set { AutoRegisterProps.SetENABLE_V(value); }
        }

        public Boolean CauseInexact
        {
            get { return AutoRegisterProps.GetCAUSE_I(); }
            set { AutoRegisterProps.SetCAUSE_I(value); }
        }

        public Boolean CauseUnderflow
        {
            get { return AutoRegisterProps.GetCAUSE_U(); }
            set { AutoRegisterProps.SetCAUSE_U(value); }
        }

        public Boolean CauseOverflow
        {
            get { return AutoRegisterProps.GetCAUSE_O(); }
            set { AutoRegisterProps.SetCAUSE_O(value); }
        }

        public Boolean CauseDivideByZero
        {
            get { return AutoRegisterProps.GetCAUSE_Z(); }
            set { AutoRegisterProps.SetCAUSE_Z(value); }
        }

        public Boolean CauseInvalidOperation
        {
            get { return AutoRegisterProps.GetCAUSE_V(); }
            set { AutoRegisterProps.SetCAUSE_V(value); }
        }

        public Boolean CauseUnimplemented
        {
            get { return AutoRegisterProps.GetCAUSE_E(); }
            set { AutoRegisterProps.SetCAUSE_E(value); }
        }

        public Boolean Condition
        {
            get { return AutoRegisterProps.GetC(); }
            set { AutoRegisterProps.SetC(value); }
        }

        public Boolean FS
        {
            get { return AutoRegisterProps.GetFS(); }
            set { AutoRegisterProps.SetFS(value); }
        }
    }
}
