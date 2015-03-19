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
        private static String[] s_OpTableRegImm =
        {
            "bltz",   "bgez",   "bltzl",   "bgezl",   null,   null, null,   null,
            "tgei",   "tgeiu",  "tlti",    "tltiu",   "teqi", null, "tnei", null,
            "bltzal", "bgezal", "bltzall", "bgezall", null,   null, null,   null,
            null,     null,     null,      null,      null,   null, null,   null,
        };

        private static OperandDictionary s_OperandLUT_RegImm = new OperandDictionary()
        {
            { 01, s_OperandFormatLUT[10] }, /* BGEZ */
            { 17, s_OperandFormatLUT[10] }, /* BGEZAL */
            { 19, s_OperandFormatLUT[10] }, /* BGEZALL */
            { 03, s_OperandFormatLUT[10] }, /* BGEZL */
            { 00, s_OperandFormatLUT[10] }, /* BLTZ */
            { 16, s_OperandFormatLUT[10] }, /* BLTZAL */
            { 18, s_OperandFormatLUT[10] }, /* BLTZALL */
            { 02, s_OperandFormatLUT[10] }, /* BLTZL */
            { 12, s_OperandFormatLUT[06] }, /* TEQI */
            { 08, s_OperandFormatLUT[06] }, /* TGEI */
            { 09, s_OperandFormatLUT[06] }, /* TGEIU */
            { 10, s_OperandFormatLUT[06] }, /* TLTI */
            { 11, s_OperandFormatLUT[06] }, /* TLTIU */
            { 14, s_OperandFormatLUT[06] }, /* TNEI */
        };
    }
}