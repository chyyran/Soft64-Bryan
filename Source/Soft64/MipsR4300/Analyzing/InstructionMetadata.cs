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

namespace Soft64.MipsR4300.Analyzing
{
    public struct InstructionMetadata
    {
        private OpcodeType m_Type;
        private OperandFormat m_Format;
        private Int64 m_Address;

        public OpcodeType InstructionType
        {
            get { return m_Type; }
            internal set { m_Type = value; }
        }

        public OperandFormat Format
        {
            get { return m_Format; }
            internal set { m_Format = value; }
        }

        public Int64 MemoryAddress
        {
            get { return m_Address; }
            internal set { m_Address = value; }
        }
    }
}