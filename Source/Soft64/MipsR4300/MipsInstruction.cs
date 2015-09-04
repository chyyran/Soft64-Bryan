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
using System.Text;
using Soft64.MipsR4300.Debugging;

namespace Soft64.MipsR4300
{
    public struct MipsInstruction
    {
        private Int64 m_Address;
        private UInt32 m_Inst;
        private Int32 m_Opcode;
        private Int32 m_Function;
        private Int32 m_Rs;
        private Int32 m_Rt;
        private Int32 m_Rd;
        private Int32 m_ShiftAmount;
        private UInt16 m_Immediate;
        private UInt32 m_Target;
        private static StringBuilder s_StringBuilderCache = new StringBuilder();

        public MipsInstruction(Int64 address, UInt32 instruction)
        {
            m_Inst = instruction;
            m_Address = address;

            unsafe
            {
                fixed (UInt32* pInstruction = (&m_Inst))
                {
                    Byte* ptr = (Byte*)pInstruction;
                    UInt16 l = *(UInt16*)(ptr + 0);
                    UInt16 h = *(UInt16*)(ptr + 2);

                    m_Immediate = l;
                    m_Target = m_Inst & 0x3FFFFFF;
                    m_Opcode = h >> 10;
                    m_Rs = h >> 5 & 0x1F;
                    m_Rt = h & 0x1F;
                    m_Rd = l >> 11;
                    m_ShiftAmount = h >> 6 & 0x1F;
                    m_Function = l & 0x3F;
                }
            }
        }

        public UInt32 Instruction { get { return m_Inst; } }

        public UInt16 Immediate { get { return m_Immediate; } }

        public UInt32 Target { get { return m_Target; } }

        public Int32 Opcode { get { return m_Opcode; } }

        public Int32 Function { get { return m_Function; } }

        public Int32 Rs { get { return m_Rs; } }

        public Int32 Rt { get { return m_Rt; } }

        public Int32 Rd { get { return m_Rd; } }

        public Int32 ShiftAmount { get { return m_ShiftAmount; } }

        public Int64 Address { get { return m_Address; } }

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
                    s_StringBuilderCache.Append(Disassembler.DecodeOperands(this, true));
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


//    public struct MipsInstruction__
//    {
//        private Int64 m_Address;
//        private UInt32 m_Inst;
//        private Byte m_Opcode;
//        private Byte m_Function;
//        private Byte m_Rs;
//        private Byte m_Rt;
//        private Byte m_Rd;
//        private Byte m_ShiftAmount;
//        private UInt16 m_Immediate;
//        private UInt32 m_Target;
//        private static StringBuilder s_StringBuilderCache = new StringBuilder();

//        public MipsInstruction__(Int64 address, UInt32 instruction)
//        {
//            m_Address = address;
//            m_Inst = instruction;
//            m_Opcode = BitExtractor.ExtractByte(instruction, 26, 6);
//            m_Target = BitExtractor.ExtractUInt32(instruction, 0, 26);
//            m_Rs = BitExtractor.ExtractByte(instruction, 21, 5);
//            m_Rt = BitExtractor.ExtractByte(instruction, 16, 5);
//            m_Immediate = BitExtractor.ExtractUInt16(instruction, 0, 16);
//            m_Rd = BitExtractor.ExtractByte(instruction, 11, 5);
//            m_Function = BitExtractor.ExtractByte(instruction, 0, 6);
//            m_ShiftAmount = BitExtractor.ExtractByte(instruction, 6, 5);
//        }

//        public Int64 Address { get { return m_Address; } }

//        public UInt32 Instruction { get { return m_Inst; } }

//        public UInt16 Immediate { get { return m_Immediate; } }

//        public UInt32 Target { get { return m_Target; } }

//        public Byte Opcode { get { return m_Opcode; } }

//        public Byte Function { get { return m_Function; } }

//        public Byte Rs { get { return m_Rs; } }

//        public Byte Rt { get { return m_Rt; } }

//        public Byte Rd { get { return m_Rd; } }

//        public Byte ShiftAmount { get { return m_ShiftAmount; } }

//        public override string ToString()
//        {
//            try
//            {
//                if (m_Inst == 0)
//                    return "nop";

//                String op = Disassembler.DecodeOpName(this);

//                if (op != null)
//                {
//                    s_StringBuilderCache.Clear();
//                    s_StringBuilderCache.Append(op);
//                    s_StringBuilderCache.Append(" ");
//                    s_StringBuilderCache.Append(Disassembler.DecodeOperands(this, true));
//                    return s_StringBuilderCache.ToString();
//                }
//                else
//                {
//                    return "";
//                }
//            }
//            catch (IndexOutOfRangeException)
//            {
//                return "IndexOutOfRangeException Thrown";
//            }
//            catch (TypeInitializationException e)
//            {
//                /* Duplicate key in some dictionary based table */
//                return e.Message;
//            }
//        }
//    }
//}