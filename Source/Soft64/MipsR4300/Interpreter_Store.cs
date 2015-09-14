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
        private static readonly UInt64[] SDLMask = 
        {
            0x0000000000000000,
            0xFF00000000000000, 
 			0xFFFF000000000000, 
 			0xFFFFFF0000000000, 
 			0xFFFFFFFF00000000, 
 			0xFFFFFFFFFF000000, 
 			0xFFFFFFFFFFFF0000, 
 			0xFFFFFFFFFFFFFF00
        };

        private static readonly Int32[] SDLShift = { 0, 8, 16, 24, 32, 40, 48, 56 };

        private static readonly UInt64[] SDRMask = 
        {
            0x00FFFFFFFFFFFFFF, 
 			0x0000FFFFFFFFFFFF, 
 			0x000000FFFFFFFFFF, 
 			0x00000000FFFFFFFF, 
 			0x0000000000FFFFFF, 
 			0x000000000000FFFF, 
 			0x00000000000000FF,  
 			0x0000000000000000
        };

        private static readonly Int32[] SDRShift = { 56, 48, 40, 32, 24, 16, 8, 0 };

        private static readonly UInt32[] SWRMask = { 0x00FFFFFF, 0x0000FFFF, 0x000000FF, 0x00000000 };

        private static readonly Int32[] SWRShift = { 24, 16, 8, 0 };

        private static readonly UInt32[] SWLMask = { 0x00000000, 0xFF000000, 0xFFFF0000, 0xFFFFFF00 };

        private static readonly Int32[] SWLShift = { 0, 8, 16, 24 };

        [OpcodeHook("SW")]
        private void Inst_Sw(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);

            if ((address & 3) != 0)
            {
                CauseException = ExceptionCode.AddressErrorStore;
            }
            else
            {

                DataManipulator.Store(address, MipsState.ReadGPR32Unsigned(inst.Rt));
            }
        }

        [OpcodeHook("SD")]
        private void Inst_Sd(MipsInstruction inst)
        {
            if (MipsState.Is32BitMode())
                throw new InvalidOperationException("Instruction reserved");

            Int64 address = (MipsState.ReadGPRSigned(inst.Rs) + (Int64)inst.Immediate).ResolveAddress();

            if ((address & 7) != 0)
            {
                CauseException = ExceptionCode.AddressErrorStore;
            }
            else
            {
                DataManipulator.Store(address, MipsState.ReadGPRUnsigned(inst.Rt));
            }
        }

        [OpcodeHook("SB")]
        private void Inst_Sb(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);
            DataManipulator.Store(address, MipsState.ReadGPRUnsigned(inst.Rt) & 0xFF);
        }

        [OpcodeHook("SC")]
        private void Inst_Sc(MipsInstruction inst)
        {
            /* This is used in atomic operations, for now let it always pass */
            MipsState.WriteGPRUnsigned(inst.Rt, 1);
            Inst_Sw(inst);
        }

        [OpcodeHook("SCD")]
        private void Inst_Scd(MipsInstruction inst)
        {
            /* This is used in atomic operations, for now let it always pass */
            MipsState.WriteGPRUnsigned(inst.Rt, 1);
            Inst_Sd(inst);
        }

        [OpcodeHook("SDL")]
        private void Inst_Sdl(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                /* Thanks to PJ64 implementation */
                Int64 address = ComputeAddress64(inst);
                Int32 offset = (Int32)(address & 7);
                UInt64 value = DataManipulator.LoadDoublewordUnsigned(address & ~7);
                value &= SDLMask[offset];
                value += MipsState.ReadGPRUnsigned(inst.Rt) >> SDLShift[offset];
                DataManipulator.Store(address & ~7, value);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("SDR")]
        private void Inst_Sdr(MipsInstruction inst)
        {
            if (MipsState.Is64BitMode())
            {
                /* Thanks to PJ64 implementation */
                Int64 address = ComputeAddress64(inst);
                Int32 offset = (Int32)(address & 7);
                UInt64 value = DataManipulator.LoadDoublewordUnsigned(address & ~7);
                value &= SDRMask[offset];
                value += MipsState.ReadGPRUnsigned(inst.Rt) << SDLShift[offset];
                DataManipulator.Store(address & ~7, value);
            }
            else
            {
                CauseException = ExceptionCode.ReservedInstruction;
            }
        }

        [OpcodeHook("SH")]
        private void Inst_Sh(MipsInstruction inst)
        {
            Int64 address = ComputeAddress64(inst);

            if ((address & 1) != 0)
            {
                CauseException = ExceptionCode.AddressErrorStore;
                return;
            }

            DataManipulator.Store(address, (UInt16)MipsState.ReadGPRUnsigned(inst.Rt));
        }

        [OpcodeHook("SWL")]
        private void Inst_Swl(MipsInstruction inst)
        {
            /* Thanks to PJ64 */
            Int64 address = ComputeAddress64(inst);
            Int32 offset = (Int32)(address & 3);
            UInt32 value = DataManipulator.LoadWordUnsigned(address & ~3);
            value &= SWLMask[offset];
            value += MipsState.ReadGPR32Unsigned(inst.Rt) >> SWLShift[offset];
            DataManipulator.Store(address & ~3, value);
        }

        [OpcodeHook("SWR")]
        private void Inst_Swr(MipsInstruction inst)
        {
            /* Thanks to PJ64 */
            Int64 address = ComputeAddress64(inst);
            Int32 offset = (Int32)(address & 3);
            UInt32 value = DataManipulator.LoadWordUnsigned(address & ~3);
            value &= SWRMask[offset];
            value += MipsState.ReadGPR32Unsigned(inst.Rt) << SWRShift[offset];
            DataManipulator.Store(address & ~3, value);
        }
    }
}