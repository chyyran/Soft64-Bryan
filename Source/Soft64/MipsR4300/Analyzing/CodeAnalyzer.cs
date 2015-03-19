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
using System.Collections.Generic;
using System.Linq;

namespace Soft64.MipsR4300.Analyzing
{
    public sealed class CodeAnalyzer
    {
        private BytecodeCache m_CodeCache;
        private Int64 m_ProgramCounter;

        public CodeAnalyzer(BytecodeCache codeCache)
        {
            m_CodeCache = codeCache;
        }

        public IEnumerable<InstructionMetadata> AnalyzeNextCodeBlock()
        {
            var results = StaticAnalyzer.DecodeCodeBlock(m_CodeCache, m_ProgramCounter);
            m_ProgramCounter += results.Count();
            return results;
        }

        public Int64 VirtualProgramCounter
        {
            get { return m_ProgramCounter; }
            set { m_ProgramCounter = value; }
        }
    }
}