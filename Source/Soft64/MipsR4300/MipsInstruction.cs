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
using System.Text;
using Soft64.MipsR4300.Debugging;

namespace Soft64.MipsR4300
{
    public struct MipsInstruction
    {
        private UInt64 m_PC;
        private UInt32 m_Inst;
        private Byte m_Opcode;
        private Byte m_Function;
        private Byte m_Rs;
        private Byte m_Rt;
        private Byte m_Rd;
        private Byte m_ShiftAmount;
        private UInt16 m_Immediate;
        private UInt32 m_Offset;
        private static StringBuilder s_StringBuilderCache = new StringBuilder();
        private Boolean m_O32Decoding;
        private DataFormat m_DataFormat;

        public MipsInstruction(UInt64 pc, UInt32 instruction)
        {
            m_PC = pc;
            m_Inst = instruction;
            m_Opcode = BitExtractor.ExtractByte(instruction, 26, 6);
            m_Offset = BitExtractor.ExtractUInt32(instruction, 0, 26);
            m_Rs = BitExtractor.ExtractByte(instruction, 21, 5);
            m_Rt = BitExtractor.ExtractByte(instruction, 16, 5);
            m_Immediate = BitExtractor.ExtractUInt16(instruction, 0, 16);
            m_Rd = BitExtractor.ExtractByte(instruction, 11, 5);
            m_Function = BitExtractor.ExtractByte(instruction, 0, 6);
            m_ShiftAmount = BitExtractor.ExtractByte(instruction, 6, 5);
            m_O32Decoding = false;
            m_DataFormat = DataFormat.Invalid;
        }

        public DataFormat DataFormat
        {
            get { return m_DataFormat; }
            set { m_DataFormat = value; }
        }

        public UInt64 PC { get { return m_PC; } }

        public UInt32 Instruction { get { return m_Inst; } }

        public UInt16 Immediate { get { return m_Immediate; } }

        public UInt32 Offset { get { return m_Offset; } }

        public Byte Opcode { get { return m_Opcode; } }

        public Byte Function { get { return m_Function; } }

        public Byte Rs { get { return m_Rs; } }

        public Byte Rt { get { return m_Rt; } }

        public Byte Rd { get { return m_Rd; } }

        public Byte ShiftAmount { get { return m_ShiftAmount; } }

        public Boolean UseO32Decoder
        {
            get { return m_O32Decoding; }
            set { m_O32Decoding = value; }
        }

        public override string ToString()
        {
            try
            {
                if (m_Inst == 0)
                    return "nop";

                String op = Disassembler.DecodeOpName(this);

                if (op != null)
                {
                    s_StringBuilderCache.Clear();
                    s_StringBuilderCache.Append(op);
                    s_StringBuilderCache.Append(" ");
                    s_StringBuilderCache.Append(Disassembler.DecodeOperands(this, m_O32Decoding));
                    return s_StringBuilderCache.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (IndexOutOfRangeException)
            {
                return "IndexOutOfRangeException Thrown";
            }
            catch (TypeInitializationException e)
            {
                /* Duplicate key in some dictionary based table */
                return e.Message;
            }
        }
    }
}