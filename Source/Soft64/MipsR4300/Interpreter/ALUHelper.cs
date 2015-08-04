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

namespace Soft64.MipsR4300.Interpreter
{
    public static class ALUHelper
    {
        public static Int32 SignExtended32(this UInt16 value)
        {
            UInt32 signedExtended = value & 0x0000FFFFU;

            /* If sign bit is 1, then set the extra high significant bits to 1 */
            if ((signedExtended & 0x00008000) != 0)
            {
                signedExtended |= 0xFFFF0000;
            }

            return (Int32)signedExtended;
        }

        public static Int64 SignExtended64(this UInt16 value)
        {
            UInt64 signedExtended = value & 0x000000000000FFFFU;

            if ((signedExtended & 0x00008000) != 0)
            {
                signedExtended |= 0xFFFFFFFFFFFF0000;
            }

            return (Int64)signedExtended;
        }

        public static UInt32 ZeroExtended32(this UInt16 value)
        {
            return 0U | value;
        }

        public static UInt64 ZeroExtended64(this UInt16 value)
        {
            return 0UL | value;
        }

        public static Int64 SignExtended64(this UInt32 value)
        {
            UInt64 signedExtended = value & 0x00000000FFFFFFFF;

            if ((signedExtended & 0x0000000080000000) != 0)
            {
                signedExtended |= 0xFFFFFFFF00000000;
            }

            return (Int64)signedExtended;
        }

        /// <summary>
        /// Resolves the signed 64 bit address into unsigned 32 bit address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static Int64 ResolveAddress(this Int64 address)
        {
            return (Int64)(UInt32)address;
        }
    }
}