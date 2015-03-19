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
using System.Runtime.CompilerServices;

namespace Soft64
{
    public static class BitExtractor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte ExtractByte(UInt64 value, Int32 offset, Int32 length)
        {
            return (Byte)(ExtractInt64(value, offset, length) & 0xFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 ExtractInt16(UInt64 value, Int32 offset, Int32 length)
        {
            return (Int16)(ExtractUInt16(value, offset, length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 ExtractInt32(UInt64 value, Int32 offset, Int32 length)
        {
            return (Int32)ExtractUInt32(value, offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ExtractUInt32(UInt64 value, Int32 offset, Int32 length)
        {
            return (UInt32)(ExtractInt64(value, offset, length) & 0xFFFFFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ExtractInt64(UInt64 value, Int32 offset, Int32 length)
        {
            return ((Int64)value >> offset) & ~(~0L << length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 ExtractUInt16(UInt64 value, Int32 offset, Int32 length)
        {
            return (UInt16)(ExtractInt64(value, offset, length) & 0xFFFF);
        }
    }
}