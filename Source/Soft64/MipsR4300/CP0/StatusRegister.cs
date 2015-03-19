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
    public sealed class StatusRegister
    {
        private _SRReg m_RegStructure;

        public StatusRegister()
        {
            m_RegStructure = default(_SRReg);
        }

        public UInt32 Reg32
        {
            get
            {
                return RegFlip(m_RegStructure.Reg);
            }
            set
            {
                m_RegStructure.Reg = RegFlip(value);
            }
        }

        public UInt64 Reg64
        {
            get { return (UInt64)Reg32; }
            set { Reg32 = (UInt32)value; }
        }

        private UInt32 RegFlip(UInt32 value)
        {
            return ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0xFF000) >> 8) | ((value & 0xFF000000) >> 24);
        }

        public Boolean InterruptEnable
        {
            get { return m_RegStructure.IE; }
            set { m_RegStructure.IE = value; }
        }

        public Boolean ExceptionLevel
        {
            get { return m_RegStructure.EXL; }
            set { m_RegStructure.EXL = value; }
        }

        public Boolean ErrorLevel
        {
            get { return m_RegStructure.ERL; }
            set { m_RegStructure.ERL = value; }
        }

        public RingMode KSUMode
        {
            get
            {
                switch (m_RegStructure.KSU & 3)
                {
                    case 0: return RingMode.Kernel;
                    case 1: return RingMode.Supervisor;
                    case 2: return RingMode.User;
                    default: return RingMode.Kernel;
                }
            }

            set
            {
                if (value < RingMode.User || value > RingMode.Kernel)
                    throw new ArgumentOutOfRangeException();

                switch (value)
                {
                    case RingMode.Kernel: m_RegStructure.KSU = 0; break;
                    case RingMode.Supervisor: m_RegStructure.KSU = 1; break;
                    case RingMode.User: m_RegStructure.KSU = 2; break;
                    default: break;
                }
            }
        }

        public Boolean UserX
        {
            get { return m_RegStructure.UX; }
            set { m_RegStructure.UX = value; }
        }

        public Boolean SupervisorX
        {
            get { return m_RegStructure.SX; }
            set { m_RegStructure.SX = value; }
        }

        public Boolean KernelX
        {
            get { return m_RegStructure.KX; }
            set { m_RegStructure.KX = value; }
        }

        public Byte InterruptMask
        {
            get { return m_RegStructure.IM; }
            set { m_RegStructure.IM = value; }
        }

        public UInt16 DiagnosticField
        {
            get { return (UInt16)(m_RegStructure.DS & 0x1FF); }
            set { m_RegStructure.DS = (UInt16)(value & 0x1FF); }
        }

        public Boolean ReducedPower
        {
            get { return m_RegStructure.RP; }
            set { m_RegStructure.RP = value; }
        }

        public Boolean FPURegsMode
        {
            get { return m_RegStructure.FR; }
            set { m_RegStructure.FR = value; }
        }

        public Boolean ReverseEndianess
        {
            get { return m_RegStructure.RE; }
            set { m_RegStructure.RE = value; }
        }

        public Byte CPUsableFlags
        {
            get { return m_RegStructure.CU; }
            set { m_RegStructure.CU = value; }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct _SRReg
    {
        [FieldOffset(0)]
        public UInt32 Reg;

        [FieldOffset(0)]
        public Boolean IE;

        [FieldOffset(1)]
        public Boolean EXL;

        [FieldOffset(2)]
        public Boolean ERL;

        [FieldOffset(3)]
        public Byte KSU;

        [FieldOffset(5)]
        public Boolean UX;

        [FieldOffset(6)]
        public Boolean SX;

        [FieldOffset(7)]
        public Boolean KX;

        [FieldOffset(8)]
        public Byte IM;

        [FieldOffset(16)]
        public UInt16 DS;

        [FieldOffset(25)]
        public Boolean RE;

        [FieldOffset(26)]
        public Boolean FR;

        [FieldOffset(27)]
        public Boolean RP;

        [FieldOffset(28)]
        public Byte CU;
    }
}