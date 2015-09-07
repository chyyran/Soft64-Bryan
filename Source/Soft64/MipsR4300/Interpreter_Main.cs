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
    public partial class Interpreter
    {
        [OpcodeHook("CACHE")]
        private void Inst_Cache(MipsInstruction inst)
        {
            logger.Debug("Cache instruction ignored");
        }

        [OpcodeHook("BREAK")]
        private void Inst_Break(MipsInstruction inst)
        {
            CauseException = ExceptionCode.Breakpoint;
        }

        [OpcodeHook("MFHI")]
        private void Inst_Mfhi(MipsInstruction inst)
        {
            MipsState.WriteGPRUnsigned(inst.Rd, MipsState.Hi);
        }

        [OpcodeHook("MFLO")]
        private void Inst_Mflo(MipsInstruction inst)
        {
            MipsState.WriteGPRUnsigned(inst.Rd, MipsState.Lo);
        }

        [OpcodeHook("MTHI")]
        private void Inst_Mthi(MipsInstruction inst)
        {
            MipsState.Hi = MipsState.ReadGPRUnsigned(inst.Rs);
        }

        [OpcodeHook("MTLO")]
        private void Inst_Mtlo(MipsInstruction inst)
        {
            MipsState.Lo = MipsState.ReadGPRUnsigned(inst.Rs);
        }

        [OpcodeHook("SYNC")]
        private void Inst_Sync(MipsInstruction inst)
        {
            /* LOL Just ignore this, we know loads and stores always finish before SYNC opcode */
            return;
        }

        [OpcodeHook("SYSCALL")]
        private void Inst_Syscall(MipsInstruction inst)
        {
            CauseException = ExceptionCode.Syscall;
        }

        [OpcodeHook("TEQ")]
        private void Inst_Teq(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) == MipsState.ReadGPR32Unsigned(inst.Rt);
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) == MipsState.ReadGPRUnsigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TEQI")]
        private void Inst_Teqi(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) == (Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) == (Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TGE")]
        private void Inst_Tge(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) >= MipsState.ReadGPR32Signed(inst.Rt);
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) >= MipsState.ReadGPRSigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TGEI")]
        private void Inst_Tgei(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) >= (Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) >= (Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TGEIU")]
        private void Inst_Tgeiu(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) >= (UInt32)(Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) >= (UInt64)(Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TGEU")]
        private void Inst_Tgeu(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) >= MipsState.ReadGPR32Unsigned(inst.Rt);
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) >= MipsState.ReadGPRUnsigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TLT")]
        private void Inst_Tlt(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) < MipsState.ReadGPR32Signed(inst.Rt);
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) < MipsState.ReadGPRSigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TLTI")]
        private void Inst_Tlti(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) < (Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) < (Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TLTIU")]
        private void Inst_Tltiu(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) < (UInt32)(Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) < (UInt64)(Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TLTU")]
        private void Inst_Tltu(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) < MipsState.ReadGPR32Unsigned(inst.Rt);
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) < MipsState.ReadGPRUnsigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TNE")]
        private void Inst_Tne(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Unsigned(inst.Rs) != MipsState.ReadGPR32Unsigned(inst.Rt);
            else
                condition = MipsState.ReadGPRUnsigned(inst.Rs) != MipsState.ReadGPRUnsigned(inst.Rt);

            if (condition)
                CauseException = ExceptionCode.Trap;
        }

        [OpcodeHook("TNEI")]
        private void Inst_Tnei(MipsInstruction inst)
        {
            Boolean condition;

            if (MipsState.Is32BitMode())
                condition = MipsState.ReadGPR32Signed(inst.Rs) != (Int32)(Int16)inst.Immediate;
            else
                condition = MipsState.ReadGPRSigned(inst.Rs) != (Int64)(Int16)inst.Immediate;

            if (condition)
                CauseException = ExceptionCode.Trap;
        }
    }
}