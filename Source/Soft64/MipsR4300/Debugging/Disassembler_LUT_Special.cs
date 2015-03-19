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
        private static String[] s_OpTableSpecial =
        {
            "sll",  null,    "srl",  "sra",  "sllv",    null,     "srlv",   "srav",
            "jr",   "jalr",  null,   null,   "syscall", "break",  null,     "sync",
            "mfhi", "mthi",  "mflo", "mtlo", "dsllv",   null,     "dsrlv",  "dsrav",
            "mult", "multu", "div",  "divu", "dmult",   "dmultu", "ddiv",   "ddivu",
            "add",  "addu",  "sub",  "subu", "and",     "or",     "xor",    "nor",
            null,   null,    "slt",  "sltu", "dadd",    "daddu",  "dsub",   "dsubu",
            "tge",  "tgeu",  "tlt",  "tltu", "teq",     null,     "tne",    null,
            "dsll", null,    "dsrl", "dsra", "dsll32",  null,     "dsrl32", "dsra32",
        };

        private static OperandDictionary s_OperandLUT_Special = new OperandDictionary()
        {
            { 00, s_OperandFormatLUT[00] }, /* SLL */
            { 02, s_OperandFormatLUT[00] }, /* SRL */
            { 03, s_OperandFormatLUT[00] }, /* SRA */
            { 56, s_OperandFormatLUT[00] }, /* DSLL */
            { 58, s_OperandFormatLUT[00] }, /* DSRL */
            { 59, s_OperandFormatLUT[00] }, /* DSRA */
            { 60, s_OperandFormatLUT[00] }, /* DSLL32 */
            { 62, s_OperandFormatLUT[00] }, /* DSRL32 */
            { 63, s_OperandFormatLUT[00] }, /* DSRA32 */
            { 25, s_OperandFormatLUT[01] }, /* MULTI */
            { 26, s_OperandFormatLUT[01] }, /* DIV */
            { 27, s_OperandFormatLUT[01] }, /* DIVU */
            { 28, s_OperandFormatLUT[01] }, /* DMULT */
            { 29, s_OperandFormatLUT[01] }, /* DMULTU */
            { 30, s_OperandFormatLUT[01] }, /* DDIV */
            { 31, s_OperandFormatLUT[01] }, /* DDIVU */
            { 16, s_OperandFormatLUT[02] }, /* MFHI */
            { 18, s_OperandFormatLUT[02] }, /* MFLO */
            { 17, s_OperandFormatLUT[03] }, /* MTHI */
            { 19, s_OperandFormatLUT[03] }, /* MTLO */
            { 22, s_OperandFormatLUT[05] }, /* DSRLV */
            { 23, s_OperandFormatLUT[03] }, /* DSRAV */
            { 15, s_OperandFormatLUT[08] }, /* SYNC */
            { 09, s_OperandFormatLUT[12] }, /* JALR */
            { 08, s_OperandFormatLUT[03] }, /* JR */
            { 13, s_OperandFormatLUT[13] }, /* BREAK */
            { 12, s_OperandFormatLUT[13] }, /* SYSCALL */
            { 52, s_OperandFormatLUT[14] }, /* TEQ */
            { 48, s_OperandFormatLUT[14] }, /* TGE */
            { 49, s_OperandFormatLUT[14] }, /* TGEU */
            { 50, s_OperandFormatLUT[14] }, /* TLT */
            { 51, s_OperandFormatLUT[14] }, /* TLTU */
            { 54, s_OperandFormatLUT[13] }, /* TNE */
            { 24, s_OperandFormatLUT[16] }, /* ERET */
        };
    }
}