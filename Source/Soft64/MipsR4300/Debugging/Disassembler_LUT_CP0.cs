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

namespace Soft64.MipsR4300.Debugging
{
    public static partial class Disassembler
    {
        private static String[] s_OpTableCop0 =
        {
            "mcf0", "dmfc0", null, null, "mtc0", "dmtc0", null, null,
            null,   null,    null,   null, null,   null,    null,   null,
            null, null,    null,   null, null,   null,    null,   null,
            null,   null,    null,   null, null,   null,    null,   null,
        };

        private static String[] s_OpTableTlb =
        {
            null,   "tlbr", "tlbwi", null, null, null, "tlbwr", null,
            "tlbp", null,   null,    null, null, null, null,    null,
            null,   null,   null,    null, null, null, null,    null,
            "eret", null,   null,    null, null, null, null,    null,
            null,   null,   null,    null, null, null, null,    null,
            null,   null,   null,    null, null, null, null,    null,
        };

        private static String[] s_OpTableBC0 =
        {
            "bc0f",  "bc0t",
            "bc0fl", "bc0tl",
        };

        private static String[] s_Cop0RegLabel =
        {
            "Index",    "Random",   "EntryLo0", "EntryLo1",
            "Context",  "PageMask", "Wired",    "Reserved",
            "BadVAddr", "Count",    "EntryHi",  "Compare",
            "Status",   "Cause",    "EPC",      "PRId",
            "Config",   "LLAddr",   "WatchLo",  "WatchHi",
            "XContext", "Reserved", "Reserved", "Reserved",
            "Reserved", "Reserved", "PErr",     "CacheErr",
            "TagLo",    "TagHi",    "ErrorEPC", "Reserved",
        };

        private static OperandDictionary s_OperandLUT_Cop0 = new OperandDictionary()
        {
            { 00, s_OperandFormatLUT[17] }, /* MFC0 */
            { 04, s_OperandFormatLUT[17] }, /* MTC0 */
        };
    }
}