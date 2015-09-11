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
                Int32 rm = AutoRegisterProps.RM;
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
                    case FPURoundMode.Near: AutoRegisterProps.RM = 0; break;
                    case FPURoundMode.Chop: AutoRegisterProps.RM = 1; break;
                    case FPURoundMode.Up: AutoRegisterProps.RM = 2; break;
                    case FPURoundMode.Down: AutoRegisterProps.RM = 3; break;
                }
            }
        }

        public Boolean FlagInexact
        {
            get { return AutoRegisterProps.FLAG_I; }
            set { AutoRegisterProps.FLAG_I = value; }
        }

        public Boolean FlagUnderflow
        {
            get { return AutoRegisterProps.FLAG_U; }
            set { AutoRegisterProps.FLAG_U = value; }
        }

        public Boolean FlagOverflow
        {
            get { return AutoRegisterProps.FLAG_O; }
            set { AutoRegisterProps.FLAG_O = value; }
        }

        public Boolean FlagDivideByZero
        {
            get { return AutoRegisterProps.FLAG_Z; }
            set { AutoRegisterProps.FLAG_Z = value; }
        }

        public Boolean FlagInvalidOperation
        {
            get { return AutoRegisterProps.FLAG_V; }
            set { AutoRegisterProps.FLAG_V = value; }
        }

        public Boolean EnableInexact
        {
            get { return AutoRegisterProps.ENABLE_I; }
            set { AutoRegisterProps.ENABLE_I = value; }
        }

        public Boolean EnableUnderflow
        {
            get { return AutoRegisterProps.ENABLE_U; }
            set { AutoRegisterProps.ENABLE_U = value; }
        }

        public Boolean EnableOverflow
        {
            get { return AutoRegisterProps.ENABLE_O; }
            set { AutoRegisterProps.ENABLE_O = value; }
        }

        public Boolean EnableDivideByZero
        {
            get { return AutoRegisterProps.ENABLE_Z; }
            set { AutoRegisterProps.ENABLE_Z = value; }
        }

        public Boolean EnableInvalidOperation
        {
            get { return AutoRegisterProps.ENABLE_V; }
            set { AutoRegisterProps.ENABLE_V = value; }
        }

        public Boolean CauseInexact
        {
            get { return AutoRegisterProps.CAUSE_I; }
            set { AutoRegisterProps.CAUSE_I = value; }
        }

        public Boolean CauseUnderflow
        {
            get { return AutoRegisterProps.CAUSE_U; }
            set { AutoRegisterProps.CAUSE_U = value; }
        }

        public Boolean CauseOverflow
        {
            get { return AutoRegisterProps.CAUSE_O; }
            set { AutoRegisterProps.CAUSE_O = value; }
        }

        public Boolean CauseDivideByZero
        {
            get { return AutoRegisterProps.CAUSE_Z; }
            set { AutoRegisterProps.CAUSE_Z = value; }
        }

        public Boolean CauseInvalidOperation
        {
            get { return AutoRegisterProps.CAUSE_V; }
            set { AutoRegisterProps.CAUSE_V = value; }
        }

        public Boolean CauseUnimplemented
        {
            get { return AutoRegisterProps.CAUSE_E; }
            set { AutoRegisterProps.CAUSE_E = value; }
        }

        public Boolean Condition
        {
            get { return AutoRegisterProps.C; }
            set { AutoRegisterProps.C = value; }
        }

        public Boolean FS
        {
            get { return AutoRegisterProps.FS; }
            set { AutoRegisterProps.FS = value; }
        }
    }
}
