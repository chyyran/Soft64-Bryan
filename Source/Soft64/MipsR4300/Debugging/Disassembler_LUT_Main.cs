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
        /* O32 Calling Convention */

        private static String[] s_O32GprLabel =
        {
            "$zero", "$at", "$v0", "$v1", "$a0", "$a1", "$a2", "$a3",
            "$t0",   "$t1", "$t2", "$t3", "$t4", "$t5", "$t6", "$t7",
            "$s0",   "$s1", "$s2", "$s3", "$s4", "$s5", "$s6", "$s7",
            "$t8",   "$t9", "$k0", "$k1", "$gp", "$sp", "$fp", "$ra",
        };

        private static String[] s_GprLabel =
        {
            "r00",  "r01", "r02", "r03", "r04", "r05", "r06", "r07",
            "r08",  "r09", "r10", "r11", "r12", "r13", "r14", "r15",
            "r16",  "r17", "r18", "r19", "r20", "r21", "r22", "r23",
            "r24",  "r25", "r26", "r27", "r28", "r29", "r30", "r31",
        };

        private static String[] s_OpTableMain =
        {
            "_SPECIAL", "_REGIMM", "j",    "jal",   "beq",  "bne",   "blez",  "bgtz",
            "addi",     "addiu",   "slti",  "sltiu", "andi", "ori",   "xori",  "lui",
            "_COP0",    "_COP1",   "_COP2", null,   "beql", "bnel",  "blezl", "bgtzl",
            "daddi",    "daddiu",  "ldl",   "ldr",   null,   null,   null,    null,
            "lb",       "lh",      "lwl",   "lw",    "lbu",  "lhu",  "lwr",   "lwu",
            "sb",       "sh",      "swl",   "sw",    "sdl",  "sdr",  "swr",   "cache",
            "ll",       "lwc1",    "lwc2",   null,   "lld",  "ldc1", "ldc2",  "ld",
            "sc",       "swc1",    "swc2",   null,   "scd",  "sdc1", "sdc2",  "sd",
        };

        public static String GetO32RegisterName(Int32 index)
        {
            return s_O32GprLabel[index];
        }

        private static OperandDictionary s_OperandLUT_Main = new OperandDictionary()
        {
            { 09, s_OperandFormatLUT[24] }, /* ADDIU */
            { 32, s_OperandFormatLUT[07] }, /* LB */
            { 36, s_OperandFormatLUT[07] }, /* LBU */
            { 55, s_OperandFormatLUT[07] }, /* LD */
            { 26, s_OperandFormatLUT[07] }, /* LDL */
            { 27, s_OperandFormatLUT[07] }, /* LDR */
            { 33, s_OperandFormatLUT[07] }, /* LH */
            { 37, s_OperandFormatLUT[07] }, /* LHU */
            { 48, s_OperandFormatLUT[07] }, /* LL */
            { 52, s_OperandFormatLUT[07] }, /* LLD */
            { 35, s_OperandFormatLUT[07] }, /* LW */
            { 34, s_OperandFormatLUT[07] }, /* LWL */
            { 38, s_OperandFormatLUT[07] }, /* LWR */
            { 39, s_OperandFormatLUT[07] }, /* LWU */
            { 40, s_OperandFormatLUT[07] }, /* SB */
            { 56, s_OperandFormatLUT[07] }, /* SC */
            { 60, s_OperandFormatLUT[07] }, /* SCD */
            { 63, s_OperandFormatLUT[07] }, /* SD */
            { 44, s_OperandFormatLUT[07] }, /* SDL */
            { 45, s_OperandFormatLUT[07] }, /* SDR */
            { 41, s_OperandFormatLUT[07] }, /* SH */
            { 43, s_OperandFormatLUT[07] }, /* SW */
            { 42, s_OperandFormatLUT[07] }, /* SWL */
            { 46, s_OperandFormatLUT[07] }, /* SWR */
            { 15, s_OperandFormatLUT[09] }, /* LUI */
            { 07, s_OperandFormatLUT[10] }, /* BGTZ */
            { 23, s_OperandFormatLUT[10] }, /* BGTZL */
            { 22, s_OperandFormatLUT[10] }, /* BLEZL */
            { 05, s_OperandFormatLUT[25] }, /* BNE */
            { 04, s_OperandFormatLUT[25] }, /* BEQ */
            { 02, s_OperandFormatLUT[11] }, /* J */
            { 03, s_OperandFormatLUT[11] }, /* JAL */
            { 47, s_OperandFormatLUT[15] }, /* CACHE */
            { 53, s_OperandFormatLUT[23] }, /* LDC1 */
            { 49, s_OperandFormatLUT[23] }, /* LWC1 */
            { 61, s_OperandFormatLUT[23] }, /* SDC1 */
            { 57, s_OperandFormatLUT[23] }, /* SWC1 */
        };
    }
}