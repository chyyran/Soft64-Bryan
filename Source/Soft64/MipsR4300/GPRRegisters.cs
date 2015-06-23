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

namespace Soft64.MipsR4300
{
    public sealed class GPRRegisters
    {
        private UInt64[] m_Registers;
        private GPRRegisters64S m_Reg64S;
        private GPRRegisters32 m_Registers32;

        public GPRRegisters()
        {
            m_Registers = new UInt64[32];
            m_Registers32 = new GPRRegisters32(this);
            m_Reg64S = new GPRRegisters64S(this);
        }

        public void Clear()
        {
            Array.Clear(m_Registers, 0, m_Registers.Length);
        }

        public GPRRegisters32 GPRRegs32
        {
            get { return m_Registers32; }
        }

        public GPRRegisters64S GPRRegs64S
        {
            get { return m_Reg64S; }
        }

        public UInt64 this[Int32 index]
        {
            get { return m_Registers[index]; }

            set
            {
                if (index > 0)
                {
                    m_Registers[index] = value;
                }
                else
                {
                    return;
                }
            }
        }

        public UInt64 GPR0
        {
            get { return m_Registers[0]; }
        }

        public UInt64 GPR1
        {
            get { return m_Registers[1]; }
            set { m_Registers[1] = value; }
        }

        public UInt64 GPR2
        {
            get { return m_Registers[2]; }
            set { m_Registers[2] = value; }
        }

        public UInt64 GPR3
        {
            get { return m_Registers[3]; }
            set { m_Registers[3] = value; }
        }

        public UInt64 GPR4
        {
            get { return m_Registers[4]; }
            set { m_Registers[4] = value; }
        }

        public UInt64 GPR5
        {
            get { return m_Registers[5]; }
            set { m_Registers[5] = value; }
        }

        public UInt64 GPR6
        {
            get { return m_Registers[6]; }
            set { m_Registers[6] = value; }
        }

        public UInt64 GPR7
        {
            get { return m_Registers[7]; }
            set { m_Registers[7] = value; }
        }

        public UInt64 GPR8
        {
            get { return m_Registers[8]; }
            set { m_Registers[8] = value; }
        }

        public UInt64 GPR9
        {
            get { return m_Registers[9]; }
            set { m_Registers[9] = value; }
        }

        public UInt64 GPR10
        {
            get { return m_Registers[10]; }
            set { m_Registers[10] = value; }
        }

        public UInt64 GPR11
        {
            get { return m_Registers[11]; }
            set { m_Registers[11] = value; }
        }

        public UInt64 GPR12
        {
            get { return m_Registers[12]; }
            set { m_Registers[12] = value; }
        }

        public UInt64 GPR13
        {
            get { return m_Registers[13]; }
            set { m_Registers[13] = value; }
        }

        public UInt64 GPR14
        {
            get { return m_Registers[14]; }
            set { m_Registers[14] = value; }
        }

        public UInt64 GPR15
        {
            get { return m_Registers[15]; }
            set { m_Registers[15] = value; }
        }

        public UInt64 GPR16
        {
            get { return m_Registers[16]; }
            set { m_Registers[16] = value; }
        }

        public UInt64 GPR17
        {
            get { return m_Registers[17]; }
            set { m_Registers[17] = value; }
        }

        public UInt64 GPR18
        {
            get { return m_Registers[18]; }
            set { m_Registers[18] = value; }
        }

        public UInt64 GPR19
        {
            get { return m_Registers[19]; }
            set { m_Registers[19] = value; }
        }

        public UInt64 GPR20
        {
            get { return m_Registers[20]; }
            set { m_Registers[20] = value; }
        }

        public UInt64 GPR21
        {
            get { return m_Registers[21]; }
            set { m_Registers[21] = value; }
        }

        public UInt64 GPR22
        {
            get { return m_Registers[22]; }
            set { m_Registers[22] = value; }
        }

        public UInt64 GPR23
        {
            get { return m_Registers[23]; }
            set { m_Registers[23] = value; }
        }

        public UInt64 GPR24
        {
            get { return m_Registers[24]; }
            set { m_Registers[24] = value; }
        }

        public UInt64 GPR25
        {
            get { return m_Registers[25]; }
            set { m_Registers[25] = value; }
        }

        public UInt64 GPR26
        {
            get { return m_Registers[26]; }
            set { m_Registers[26] = value; }
        }

        public UInt64 GPR27
        {
            get { return m_Registers[27]; }
            set { m_Registers[27] = value; }
        }

        public UInt64 GPR28
        {
            get { return m_Registers[28]; }
            set { m_Registers[28] = value; }
        }

        public UInt64 GPR29
        {
            get { return m_Registers[29]; }
            set { m_Registers[29] = value; }
        }

        public UInt64 GPR30
        {
            get { return m_Registers[30]; }
            set { m_Registers[30] = value; }
        }

        public UInt64 GPR31
        {
            get { return m_Registers[31]; }
            set { m_Registers[31] = value; }
        }
    }

    public sealed class GPRRegisters32
    {
        private GPRRegisters m_ParentGPRRef;
        private GPRRegisters32S m_SignedRegs;

        internal GPRRegisters32(GPRRegisters parentRegs)
        {
            m_ParentGPRRef = parentRegs;
            m_SignedRegs = new GPRRegisters32S(this);
        }

        public UInt32 this[Int32 index]
        {
            get { unchecked { return (UInt32)(m_ParentGPRRef[index] & 0x00000000FFFFFFFF); } }
            set { unchecked { m_ParentGPRRef[index] = value; } }
        }

        public GPRRegisters32S GPRRegsSigned32
        {
            get { return m_SignedRegs; }
        }
    }

    public sealed class GPRRegisters32S
    {
        private GPRRegisters32 m_Regs;

        internal GPRRegisters32S(GPRRegisters32 regs)
        {
            m_Regs = regs;
        }

        public Int32 this[Int32 index]
        {
            get { return (Int32)m_Regs[index]; }
            set { m_Regs[index] = (UInt32)value; }
        }
    }

    public sealed class GPRRegisters64S
    {
        private GPRRegisters m_Regs;

        internal GPRRegisters64S(GPRRegisters regs)
        {
            m_Regs = regs;
        }

        public Int64 this[Int32 index]
        {
            get { return (Int64)m_Regs[index]; }
            set { m_Regs[index] = (UInt64)value; }
        }
    }
}