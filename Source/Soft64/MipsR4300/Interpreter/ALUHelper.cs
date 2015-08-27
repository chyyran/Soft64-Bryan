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
        /// <summary>
        /// Resolves the signed 64 bit address into unsigned 32 bit address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static Int64 ResolveAddress(this Int64 address)
        {
            return (Int64)(UInt32)address;
        }

        public static Boolean IsSigned32(this UInt64 value)
        {
            UInt32 val = (UInt32)(value >> 32);
            return  val == 0xFFFFFFFF || val == 0x00000000;
        }
    }
}