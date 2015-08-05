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
using System.IO;
using Soft64.IO;
using Soft64.MipsR4300.IO;

namespace Soft64.MipsR4300.Interpreter
{
    public abstract class BaseInterpreter : ExecutionEngine
    {
        private Stream m_VMStream;          /* Virtual Memory Stream */
        private Stream m_BigEndianVMStream; /* Big-Endian ordered virtual memory stream */
        private BinaryReader m_InstructionBinReader;
        private BinaryReader m_DataBinReader;
        private BinaryWriter m_DataBinWriter;
        private ExecutionState m_CPUState;
        private Boolean m_IsHLEKernel;

        public BaseInterpreter()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_CPUState = ParentMips.State;
            m_VMStream = ParentMips.VirtualMemoryStream;
            m_BigEndianVMStream = new Int32SwapStream(m_VMStream);
            m_InstructionBinReader = new BinaryReader(m_BigEndianVMStream);
            m_DataBinReader = new BinaryReader(m_VMStream);
            m_DataBinWriter = new BinaryWriter(m_VMStream);
        }

        public abstract override void Step();

        protected ExecutionState MipsState
        {
            get { return m_CPUState; }
        }

        protected BinaryReader InstructionBinaryReader
        {
            get { return m_InstructionBinReader; }
        }

        protected BinaryReader DataBinaryReader
        {
            get { return m_DataBinReader; }
        }

        protected BinaryWriter DataBinaryWriter
        {
            get { return m_DataBinWriter; }
        }

        protected Int64 VMemoryPosition
        {
            get { return m_VMStream.Position; }
            set { m_VMStream.Position = value; }
        }

        protected MipsInstruction FetchInstruction(Int64 address)
        {
            m_InstructionBinReader.BaseStream.Position = address;
            return new MipsInstruction((UInt64)address, m_InstructionBinReader.ReadUInt32());
        }
    }
}