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
        private static String[] s_OpTableCop1 =
        {
            "mfc1", "dmfc1", "cfc1", null, "mtc1", "dmtc1", "ctc1", null,
            "_BC1", null,    null,   null, null,   null,    null,   null,
            "_FPU", "_FPU",    null,   null, "_FPU",  "_FPU",   null,   null,
            null,  null,     null,   null, null,   null,    null,   null,
        };

        private static String[] s_OpTableBC1 =
        {
            "bc1f",  "bc1t",
            "bc1fl", "bc1tl",
        };

        private static String[] s_OpTableFpu =
        {
            "add.fmt",     "sub.fmt",     "mul.fmt",    "div.fmt",     "sqrt.fmt",    "abs.fmt",     "mov.fmt",    "neg.fmt",
            "round.l.fmt", "trunc.l.fmt", "ceil.l.fmt", "floor.l.fmt", "round.w.fmt", "trunc.w.fmt", "ceil.w.fmt", "floor.w.fmt",
            null,      null,      null,     null,      null,      null,      null,     null,
            null,      null,      null,     null,      null,      null,      null,     null,
            "cvt.s.fmt",   "cvt.d.fmt",   null,     null,      "cvt.w.fmt",   "cvt.l.fmt",   null,     null,
            null,      null,      null,     null,      null,      null,      null,     null,
            "c.cond.fmt",     "c.cond.fmt",    "c.cond.fmt",   "c.cond.fmt",   "c.cond.fmt",   "c.cond.fmt",   "c.cond.fmt",  "c.cond.fmt",
            "c.cond.fmt",    "c.cond.fmt",  "c.cond.fmt",  "c.cond.fmt",   "c.cond.fmt",    "c.cond.fmt",   "c.cond.fmt",   "c.cond.fmt",
        };

        private static String[] s_Cop1RegLabel =
        {
            "fpr0",  "fpr1",  "fpr2",  "fpr3",  "fpr4",  "fpr5",  "fpr6",  "fpr7",
            "fpr8",  "fpr9",  "fpr10", "fpr11", "fpr12", "fpr13", "fpr14", "fpr15",
            "fpr16", "fpr17", "fpr18", "fpr19", "fpr20", "fpr21", "fpr22", "fpr23",
            "fpr24", "fpr25", "fpr26", "fpr27", "fpr28", "fpr29", "fpr30", "fpr31",
        };

        private static OperandDictionary s_OperandLUT_Cop1 = new OperandDictionary()
        {
            { 02, s_OperandFormatLUT[22] }, /* CFC1 */
            { 06, s_OperandFormatLUT[22] }, /* CTC1 */
            { 01, s_OperandFormatLUT[22] }, /* DMFC1 */
            { 05, s_OperandFormatLUT[22] }, /* DMTC1 */
            { 04, s_OperandFormatLUT[22] }, /* MFC1 */
        };

        private static OperandDictionary s_OperandLUT_FPU = new OperandDictionary()
        {
            { 05, s_OperandFormatLUT[19] }, /* ABS.fmt */
            { 00, s_OperandFormatLUT[20] }, /* ADD.fmt */
            { 48, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 49, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 50, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 51, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 52, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 53, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 54, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 55, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 56, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 57, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 58, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 59, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 60, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 61, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 62, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 63, s_OperandFormatLUT[21] }, /* C.cond.fmt */
            { 10, s_OperandFormatLUT[19] }, /* CEIL.L.fmt */
            { 14, s_OperandFormatLUT[19] }, /* CEIL.W.fmt */
            { 33, s_OperandFormatLUT[19] }, /* CVT.D.fmt */ // TODO: Only in D Table
            { 37, s_OperandFormatLUT[19] }, /* CVT.L.fmt */
            { 32, s_OperandFormatLUT[19] }, /* CVT.S.fmt */
            { 36, s_OperandFormatLUT[19] }, /* CVT.W.fmt */
            { 03, s_OperandFormatLUT[19] }, /* DIV.fmt */
            { 11, s_OperandFormatLUT[19] }, /* FLOOR.L.fmt */
            { 15, s_OperandFormatLUT[19] }, /* FLOOR.W.fmt */
            { 02, s_OperandFormatLUT[20] }, /* MUL.fmt */
            { 07, s_OperandFormatLUT[19] }, /* NEG.fmt */
            { 08, s_OperandFormatLUT[19] }, /* ROUND.L.fmt */
            { 12, s_OperandFormatLUT[19] }, /* ROUND.W.fmt */
            { 04, s_OperandFormatLUT[19] }, /* SQRT.fmt */
            { 01, s_OperandFormatLUT[20] }, /* SUB.fmt */
            { 09, s_OperandFormatLUT[19] }, /* TRUNC.L.fmt */
            { 13, s_OperandFormatLUT[19] }, /* TRUNC.W.fmt */
        };
    }
}