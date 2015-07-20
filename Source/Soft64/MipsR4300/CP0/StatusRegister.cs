/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Runtime.InteropServices;

namespace Soft64.MipsR4300.CP0
{
    [RegisterField("IE", 1, 0, typeof(Boolean))]
    [RegisterField("EXL", 1, 1, typeof(Boolean))]
    [RegisterField("ERL", 1, 2, typeof(Boolean))]
    [RegisterField("KSU", 2, 3, typeof(Byte))]
    [RegisterField("UX", 1, 5, typeof(Boolean))]
    [RegisterField("SX", 1, 6, typeof(Boolean))]
    [RegisterField("KX", 1, 7, typeof(Boolean))]
    [RegisterField("IM0", 1, 8, typeof(Boolean))]
    [RegisterField("IM1", 1, 9, typeof(Boolean))]
    [RegisterField("IM2", 1, 10, typeof(Boolean))]
    [RegisterField("IM3", 1, 11, typeof(Boolean))]
    [RegisterField("IM4", 1, 12, typeof(Boolean))]
    [RegisterField("IM5", 1, 13, typeof(Boolean))]
    [RegisterField("IM6", 1, 14, typeof(Boolean))]
    [RegisterField("IM7", 1, 15, typeof(Boolean))]
    [RegisterField("BEV", 1, 22, typeof(Boolean))]
    [RegisterField("TS", 1, 21, typeof(Boolean))]
    [RegisterField("SR", 1, 20, typeof(Boolean))]
    [RegisterField("CH", 1, 18, typeof(Boolean))]
    [RegisterField("CE", 1, 17, typeof(Boolean))]
    [RegisterField("DE", 1, 16, typeof(Boolean))]
    [RegisterField("RE", 1, 25, typeof(Boolean))]
    [RegisterField("FR", 1, 26, typeof(Boolean))]
    [RegisterField("RP", 1, 27, typeof(Boolean))]
    [RegisterField("CU0", 1, 28, typeof(Boolean))]
    [RegisterField("CU1", 1, 29, typeof(Boolean))]
    [RegisterField("CU2", 1, 30, typeof(Boolean))]
    [RegisterField("CU3", 1, 31, typeof(Boolean))]
    public sealed class StatusRegister : SmartRegister<UInt32>
    {
        public StatusRegister() : base()
        {
        }

        public Boolean CopUsable0
        {
            get { return AutoRegisterProps.GetCU0(); }
            set { AutoRegisterProps.SetCU0(value); }
        }

        public Boolean CopUsable1
        {
            get { return AutoRegisterProps.GetCU1(); }
            set { AutoRegisterProps.SetCU1(value); }
        }

        public Boolean CopUsable2
        {
            get { return AutoRegisterProps.GetCU2(); }
            set { AutoRegisterProps.SetCU2(value); }
        }

        public Boolean CopUsable3
        {
            get { return AutoRegisterProps.GetCU3(); }
            set { AutoRegisterProps.SetCU3(value); }
        }

        public Boolean ReducePower
        {
            get { return AutoRegisterProps.GetRP(); }
            set { AutoRegisterProps.SetRP(value); }
        }

        public Boolean AdditionalFPR
        {
            get { return AutoRegisterProps.GetFR(); }
            set { AutoRegisterProps.SetFR(value); }
        }

        public Boolean ReverseEndianess
        {
            get { return AutoRegisterProps.GetRE(); }
            set { AutoRegisterProps.SetRE(value); }
        }

        public Boolean ExceptionVectorMode
        {
            get { return AutoRegisterProps.GetBEV(); }
            set { AutoRegisterProps.SetBEV(value); }
        }

        public Boolean TlbShutdown
        {
            get { return AutoRegisterProps.GetTS(); }
            set { AutoRegisterProps.SetTS(value); }
        }

        public Boolean ResetMode
        {
            get { return AutoRegisterProps.GetSR(); }
            set { AutoRegisterProps.SetSR(value); }
        }

        public Boolean CacheHit
        {
            get { return AutoRegisterProps.GetCH(); }
            set { AutoRegisterProps.SetCH(value); }
        }

        public Boolean EccCheckBits
        {
            get { return AutoRegisterProps.GetCE(); }
            set { AutoRegisterProps.SetCE(value); }
        }

        public Boolean DE
        {
            get { return AutoRegisterProps.GetDE(); }
            set { AutoRegisterProps.SetDE(value); }
        }

        public Boolean InterruptMask0
        {
            get { return AutoRegisterProps.GetIM0(); }
            set { AutoRegisterProps.SetIM0(value); }
        }

        public Boolean InterruptMask1
        {
            get { return AutoRegisterProps.GetIM1(); }
            set { AutoRegisterProps.SetIM1(value); }
        }

        public Boolean InterruptMask2
        {
            get { return AutoRegisterProps.GetIM2(); }
            set { AutoRegisterProps.SetIM2(value); }
        }

        public Boolean InterruptMask3
        {
            get { return AutoRegisterProps.GetIM3(); }
            set { AutoRegisterProps.SetIM3(value); }
        }

        public Boolean InterruptMask4
        {
            get { return AutoRegisterProps.GetIM4(); }
            set { AutoRegisterProps.SetIM4(value); }
        }

        public Boolean InterruptMask5
        {
            get { return AutoRegisterProps.GetIM5(); }
            set { AutoRegisterProps.SetIM5(value); }
        }

        public Boolean InterruptMask6
        {
            get { return AutoRegisterProps.GetIM6(); }
            set { AutoRegisterProps.SetIM6(value); }
        }

        public Boolean InterruptMask7
        {
            get { return AutoRegisterProps.GetIM7(); }
            set { AutoRegisterProps.SetIM7(value); }
        }

        public Boolean KernelAddressingMode
        {
            get { return AutoRegisterProps.GetKX(); }
            set { AutoRegisterProps.SetKX(value); }
        }

        public Boolean SupervisorAddressingMode
        {
            get { return AutoRegisterProps.GetSX(); }
            set { AutoRegisterProps.SetSX(value); }
        }

        public Boolean UserAddressingMode
        {
            get { return AutoRegisterProps.GetUX(); }
            set { AutoRegisterProps.SetUX(value); }
        }

        public Byte KSU
        {
            get { return AutoRegisterProps.GetKSU(); }
            set { AutoRegisterProps.SetKSU(value); }
        }

        public RingMode RingFlags
        {
            get
            {
                switch (KSU)
                {
                    default:
                    case 0: return RingMode.Kernel;
                    case 1: return RingMode.Supervisor;
                    case 2: return RingMode.User;
                }
            }

            set
            {
                switch (value)
                {
                    default:
                    case RingMode.Kernel: KSU = 0; break;
                    case RingMode.Supervisor: KSU = 1; break;
                    case RingMode.User: KSU = 2; break;
                }
            }
        }

        public Boolean ErrorLevel
        {
            get { return AutoRegisterProps.GetERL(); }
            set { AutoRegisterProps.SetERL(value); }
        }

        public Boolean ExceptionLevel
        {
            get { return AutoRegisterProps.GetEXL(); }
            set { AutoRegisterProps.SetEXL(value); }
        }

        public Boolean EnableInterrupts
        {
            get { return AutoRegisterProps.GetIE(); }
            set { AutoRegisterProps.SetIE(value); }
        }

        public UInt64 RegisterValue64
        {
            get { return RegisterValue; }
            set { RegisterValue = (UInt32)value; }
        }

        private UInt32 RegFlip(UInt32 value)
        {
            return ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0xFF000) >> 8) | ((value & 0xFF000000) >> 24);
        }
    }
}