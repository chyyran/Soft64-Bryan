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

using Soft64.MipsR4300;
using Soft64.MipsR4300.CP1;

namespace Soft64.MipsR4300
{
    /// <summary>
    /// Stores the low level internal state of the MIPS R4300I microprocessor.
    /// </summary>
    [Serializable]
    public sealed class ExecutionState
    {
        private GPRRegisters m_Regs = new GPRRegisters();
        private CoreClock m_Clock = new CoreClock();

        /// <summary>
        /// Specifies which mode the CPU is running in.  If true, then its running in 64bit else 32bit.
        /// </summary>
        public Boolean Mode { get; set; }

        /// <summary>
        /// General purpose registers. Based on the CPU mode, they are either 32bit or 64bit.
        /// <remarks>
        /// R0 is a wired value of 0.
        /// R31 is used by jump/link instructions, nothing else should use this.
        /// </remarks>
        /// </summary>
        public GPRRegisters GPRRegs { get { return m_Regs; }}

        public UInt64 ReadGPRUnsigned(Int32 index)
        {
            return m_Regs[index];
        }

        public void WriteGPRUnsigned(Int32 index, UInt64 value)
        {
            m_Regs[index] = value;
        }

        public Int64 ReadGPRSigned(Int32 index)
        {
            return (Int64)m_Regs[index];
        }

        public void WriteGPRSigned(Int32 index, Int64 value)
        {
            m_Regs[index] = (UInt64)value;
        }

        public UInt32 ReadGPR32Unsigned(Int32 index)
        {
            return (UInt32)m_Regs[index];
        }

        public void WriteGPR32Unsigned(Int32 index, UInt32 value)
        {
            m_Regs[index] = value;
        }

        public Int32 ReadGPR32Signed(Int32 index)
        {
            return (Int32)(UInt32)m_Regs[index];
        }

        public void WriteGPR32Signed(Int32 index, Int32 value)
        {
            m_Regs[index] = (UInt32)value;
        }

        /// <summary>
        /// Register for storing the higher result of Multiplication/Division computations.
        /// <remarks>
        /// Integer multiplication products are stored in both registers or remainer is stored here.
        /// </remarks>
        /// </summary>

        public UInt64 Hi { get; set; }

        /// <summary>
        /// Register that stores the lower result of Multiplication/Division computations.
        /// <remarks>
        /// Quotient is stored here.
        /// </remarks>
        /// </summary>

        public UInt64 Lo { get; set; }

        /// <summary>
        /// Program counter
        /// </summary>

        public Int64 PC { get; set; }

        /// <summary>
        /// A bit used during synchronization.
        /// </summary>
        public Boolean LLBit { get; set; }

        /// <summary>
        /// System Control Processor Registers
        /// </summary>
        public CP0Registers CP0Regs { get; private set; }

        /// <summary>
        /// Floating-Point General Purpose Registers. These registers are part of the FPU.
        /// </summary>
        public FpuRegisters Fpr { get; private set; }

        /// <summary>
        /// FPU Implementation/Revision Register
        /// </summary>
        public UInt32 FCR0 { get; set; }

        /// <summary>
        /// FPU Status and Control register
        /// </summary>
        public Fcr32Register FCR31 { get; private set; }

        public Boolean NullifyEnabled { get; set; }

        public Boolean BranchEnabled { get; set; }

        public Int64 BranchTarget { get; set; }

        public Int64 BranchDelaySlot { get; set; }

        public CoreClock Clock { get; set; }

        public Int64 BranchPC
        {
            get;
            set;
        }

        public Boolean Operating64BitMode
        {
            get
            {
                if (CP0Regs.StatusReg.RingFlags == RingMode.Kernel)
                {
                    return true;
                }
                else if (CP0Regs.StatusReg.RingFlags == RingMode.Supervisor)
                {
                    return CP0Regs.StatusReg.SupervisorAddressingMode;
                }
                else
                {
                    return CP0Regs.StatusReg.UserAddressingMode;
                }
            }
        }

        public Boolean Addressing64BitMode
        {
            get
            {
                if (CP0Regs.StatusReg.RingFlags == RingMode.Kernel)
                {
                    return CP0Regs.StatusReg.KernelAddressingMode;
                }
                else if (CP0Regs.StatusReg.RingFlags == RingMode.Supervisor)
                {
                    return CP0Regs.StatusReg.SupervisorAddressingMode;
                }
                else
                {
                    return CP0Regs.StatusReg.UserAddressingMode;
                }
            }
        }

        public ExecutionState()
        {
            FCR31 = new Fcr32Register();
            CP0Regs = new CP0Registers();
            Fpr = new FpuRegisters();
            m_Regs = new GPRRegisters();
        }
    }
}