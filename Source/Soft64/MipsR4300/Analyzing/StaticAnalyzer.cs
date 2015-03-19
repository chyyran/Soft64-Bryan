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
    public static class StaticAnalyzer
    {
        public static IEnumerable<InstructionMetadata> DecodeCodeBlock(BytecodeCache cache, Int64 memoryAddress)
        {
            var blockOffset = 0;
            var codeBlock = cache.FetchBytecodeBlock(memoryAddress, out blockOffset);
            var codeQuery = QueryCodeRuns(codeBlock, blockOffset, cache.BlockSize);
            var jumpIndex = codeQuery.Last() + 4;

            return
                (from offset in Enumerable.Concat<Int32>(codeQuery, new Int32[] { jumpIndex })
                 let instType = DecodeType(codeBlock, offset)
                 let format = DecodeFormat(codeBlock, offset)
                 select
                     new InstructionMetadata
                     {
                         InstructionType = instType,
                         Format = format,
                         MemoryAddress = memoryAddress + offset
                     })
                    .ToArray();
        }

        public static IEnumerable<Int32> QueryCodeRuns(Byte[] codeBlock, Int32 offset, Int32 length)
        {
            return
                (from bufferIndex in QueryBytecodeBufferIndicies(offset, length)
                 select new { IsRun = !OpcodeIsJump(codeBlock, bufferIndex), Index = bufferIndex })
                 .TakeWhile((data) => data.IsRun)
                 .Select((data) => data.Index)
                 .ToArray();
        }

        public static OpcodeType DecodeType(Byte[] buffer, Int32 offset)
        {
            return OpcodeType.Invalid;
        }

        public static OperandFormat DecodeFormat(Byte[] buffer, Int32 offset)
        {
            return OperandFormat.Invalid;
        }

        public static Boolean OpcodeIsJump(Byte[] buffer, Int32 offset)
        {
            return false;
        }

        private static IEnumerable<Int32> QueryBytecodeBufferIndicies(Int32 offset, Int32 blockSize)
        {
            return (
                from index in Enumerable.Range(offset, blockSize / 4)
                select index * 4).ToArray();
        }
    }
}