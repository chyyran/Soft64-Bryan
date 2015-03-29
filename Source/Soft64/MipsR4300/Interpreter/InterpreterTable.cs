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
using NLog;
using MipsOp = System.Action<Soft64.MipsR4300.MipsInstruction>;

namespace Soft64.MipsR4300.Interpreter
{
    public sealed class InterpreterTable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Int32 m_OpcodeErrorCount = 0;

        private MipsOp m_SubSpecial;
        private MipsOp m_SubRegImm;
        private MipsOp m_COP0;
        private MipsOp m_COP1;
        private MipsOp m_COP2;
        private MipsOp m_BC1;
        private MipsOp m_SI;
        private MipsOp m_DI;
        private MipsOp m_WI;
        private MipsOp m_LI;
        private MipsOp m_TLB;

        /* Call Tables */
        private MipsOp[] m_OpsTableMain;
        private MipsOp[] m_OpsTableSpecial;
        private MipsOp[] m_OpsTableRegImm;
        private MipsOp[] m_OpsTableCP0;
        private MipsOp[] m_OpsTableCP1;
        private MipsOp[] m_OpsTableCP2;
        private MipsOp[] m_OpsTableBC1;
        private MipsOp[] m_OpsTableFloat;
        private MipsOp[] m_OpsTableFixed;
        private MipsOp[] m_OpsTableTLB;

        public void InitializeCallTables()
        {
            m_SubSpecial = (inst) => OpCall(m_OpsTableSpecial, inst.Function, inst);
            m_SubRegImm = (inst) => OpCall(m_OpsTableRegImm, inst.Rt, inst);
            m_COP0 = (inst) => OpCall(m_OpsTableCP0, inst.Rs, inst);
            m_COP1 = (inst) => OpCall(m_OpsTableCP1, inst.Rs, inst);
            m_TLB = (inst) => OpCall(m_OpsTableTLB, inst.Function, inst);
            m_BC1 = (inst) => OpCall(m_OpsTableBC1, inst.Rt & 0x3, inst);

            m_WI = (inst) => { inst.DataFormat = DataFormat.FixedWord; OpCall(m_OpsTableFixed, inst.Function, inst); };
            m_LI = (inst) => { inst.DataFormat = DataFormat.FixedLong; OpCall(m_OpsTableFixed, inst.Function, inst); };
            m_SI = (inst) => { inst.DataFormat = DataFormat.FloatingSingle; OpCall(m_OpsTableFloat, inst.Function, inst); };
            m_DI = (inst) => { inst.DataFormat = DataFormat.FloatingDouble; OpCall(m_OpsTableFloat, inst.Function, inst); };

            m_OpsTableMain = new MipsOp[] {
                m_SubSpecial, m_SubRegImm, J, JAL, BEQ, BNE, BLEZ, BGTZ,
                ADDI, ADDIU, SLTI, SLTIU, ANDI, ORI, XORI, LUI,
                m_COP0, m_COP1, m_COP2, null, BEQL, BNEL, BLEZL, BGTZL,
                DADDI, DADDIU, LDL, LDR, null, null, null, null,
                LB, LH, LWL, LW, LBU, LHU, LWR, LWU,
                SB, SH, SWL, SW, SDL, SDR, SWR, CACHE,
                LL, LWC1, LWC2, null, LLD, LDC1, LDC2, LD,
                SC, SWC1, SWC2, null, SCD, SDC1, SDC2, SD
            };

            m_OpsTableSpecial = new MipsOp[] {
                SLL, null, SRL, SRA, SLLV, null, SRLV, SRAV,
                JR, JALR, null, null, SYSCALL, BREAK, null, SYNC,
                MFHI, MTHI, MFLO, MTLO, DSLLV, null, DSRLV, DSRAV,
                MULT, MULTU, DIV, DIVU, DMULT, DMULTU, DDIV, DDIVU,
                ADD, ADDU, SUB, SUBU, AND, OR, XOR, NOR,
                null, null, SLT, SLTU, DADD, DADDU, DSUB, DSUBU,
                TGE, TGEU, TLT, TLTU, TEQ, null, TNE, null,
                DSLL, null, DSRL, DSRA, DSLL32, null, DSRL32, DSRA32
            };

            m_OpsTableRegImm = new MipsOp[] {
                BLTZ, BGEZ, BLTZL, BGEZL, null, null, null, null,
                TGEI, TGEIU, TLTI, TLTIU, TEQI, null, TNEI, null,
                BLTZAL, BGEZAL, BLTZALL, BGEZALL, null, null, null, null,
                null, null, null, null, null, null, null, null
            };

            m_OpsTableCP0 = new MipsOp[] {
                MCF0, DMFC0, CFC0, null, MTC0, DMTC0, CTC0, null,
                null, null, null, null, null, null, null, null,
                m_TLB, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null
            };

            m_OpsTableTLB = new MipsOp[] {
                null, TLBR, TLBWI, null, null, null, TLBWR, null,
                TLBP, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                ERET, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null
            };

            m_OpsTableCP1 = new MipsOp[] {
                MFC1, DMFC1, CFC1, null, MTC1, DMTC1, CTC1, null,
                m_BC1, null, null, null, null, null, null, null,
                m_SI, m_DI, null, null, m_WI, m_LI, null, null,
                null, null, null, null, null, null, null, null
            };

            m_OpsTableBC1 = new MipsOp[] {
                BC1F, BC1T, BC1FL, BC1TL
            };

            m_OpsTableFloat = new MipsOp[] {
                FPU_ADD, FPU_SUB, FPU_MUL, FPU_DIV, FPU_SQRT, FPU_ABS, FPU_MOV, FPU_NEG,
                FPU_ROUND_L, FPU_TRUNC_L, FPU_CEIL_L, FPU_FLOOR_L, FPU_ROUND_W, FPU_TRUNC_W, FPU_CEIL_W, FPU_FLOOR_W,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                FPU_CVT_S, FPU_CVT_D, null, null, FPU_CVT_W, FPU_CVT_L, null, null,
                null, null, null, null, null, null, null, null,
                FPU_C_F, FPU_C_UN, FPU_C_EQ, FPU_C_UEQ, FPU_C_UEQ, FPU_C_OLT, FPU_C_ULT, FPU_C_OLE, FPU_C_ULE,
                FPU_C_SF, FPU_C_NGLE, FPU_C_SEQ, FPU_C_NGL, FPU_C_LT, FPU_C_NGE, FPU_C_LE, FPU_C_NGT
            };
        }

        public InterpreterTable()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OpCall(MipsOp[] callTable, Int32 index, MipsInstruction inst)
        {
            if (inst.Instruction == 0)
                return;

#if !FAST_UNSAFE_BUILD
            try
            {
                MipsOp e = callTable[index];

                if (logger.IsDebugEnabled)
                    logger.Debug("{0:X8} {1:X8} {2}", Machine.Current.CPU.State.PC, inst.Instruction, inst.ToString());

                if (e != null)
                    e(inst);
                else
                    throw new InvalidOperationException("Unsupport Instruction: " + inst.ToString());
            }
            catch (OverflowException e)
            {
#if DEBUG
                Console.WriteLine("Overflow at " + e.TargetSite.Name);
#endif
                return;
            }
            catch (Exception e)
            {
                m_OpcodeErrorCount++;
                Console.WriteLine("Interpreter Opcode Error: " + e.Message);

                if (m_OpcodeErrorCount >= 1)
                    throw new InvalidOperationException("Too many opcode errors have occured");

                return;
            }
#else
            callTable[index](inst);
#endif
        }

        public void CallInstruction(MipsInstruction inst)
        {
            OpCall(m_OpsTableMain, inst.Opcode, inst);
        }

        public MipsOp J { get; set; }

        public MipsOp JAL { get; set; }

        public MipsOp BEQ { get; set; }

        public MipsOp BNE { get; set; }

        public MipsOp BLEZ { get; set; }

        public MipsOp BGTZ { get; set; }

        public MipsOp ADDI { get; set; }

        public MipsOp ADDIU { get; set; }

        public MipsOp SLTI { get; set; }

        public MipsOp SLTIU { get; set; }

        public MipsOp ANDI { get; set; }

        public MipsOp ORI { get; set; }

        public MipsOp XORI { get; set; }

        public MipsOp LUI { get; set; }

        public MipsOp BEQL { get; set; }

        public MipsOp BNEL { get; set; }

        public MipsOp BLEZL { get; set; }

        public MipsOp BGTZL { get; set; }

        public MipsOp DADDI { get; set; }

        public MipsOp DADDIU { get; set; }

        public MipsOp LDL { get; set; }

        public MipsOp LDR { get; set; }

        public MipsOp LB { get; set; }

        public MipsOp LH { get; set; }

        public MipsOp LWL { get; set; }

        public MipsOp LW { get; set; }

        public MipsOp LBU { get; set; }

        public MipsOp LWR { get; set; }

        public MipsOp LWU { get; set; }

        public MipsOp SB { get; set; }

        public MipsOp SH { get; set; }

        public MipsOp SWL { get; set; }

        public MipsOp SW { get; set; }

        public MipsOp SDL { get; set; }

        public MipsOp SDR { get; set; }

        public MipsOp SWR { get; set; }

        public MipsOp CACHE { get; set; }

        public MipsOp LL { get; set; }

        public MipsOp LWC1 { get; set; }

        public MipsOp LWC2 { get; set; }

        public MipsOp LLD { get; set; }

        public MipsOp LDC1 { get; set; }

        public MipsOp LDC2 { get; set; }

        public MipsOp LD { get; set; }

        public MipsOp SC { get; set; }

        public MipsOp SWC1 { get; set; }

        public MipsOp SWC2 { get; set; }

        public MipsOp SCD { get; set; }

        public MipsOp SDC1 { get; set; }

        public MipsOp SDC2 { get; set; }

        public MipsOp SD { get; set; }

        public MipsOp SLL { get; set; }

        public MipsOp SRL { get; set; }

        public MipsOp SRA { get; set; }

        public MipsOp SLLV { get; set; }

        public MipsOp SRLV { get; set; }

        public MipsOp SRAV { get; set; }

        public MipsOp JR { get; set; }

        public MipsOp JALR { get; set; }

        public MipsOp SYSCALL { get; set; }

        public MipsOp BREAK { get; set; }

        public MipsOp SYNC { get; set; }

        public MipsOp MFHI { get; set; }

        public MipsOp MTHI { get; set; }

        public MipsOp MFLO { get; set; }

        public MipsOp MTLO { get; set; }

        public MipsOp DSLLV { get; set; }

        public MipsOp DSRLV { get; set; }

        public MipsOp DSRAV { get; set; }

        public MipsOp MULT { get; set; }

        public MipsOp DIV { get; set; }

        public MipsOp DIVU { get; set; }

        public MipsOp DMULT { get; set; }

        public MipsOp DMULTU { get; set; }

        public MipsOp DDIV { get; set; }

        public MipsOp DDIVU { get; set; }

        public MipsOp ADD { get; set; }

        public MipsOp ADDU { get; set; }

        public MipsOp SUB { get; set; }

        public MipsOp SUBU { get; set; }

        public MipsOp AND { get; set; }

        public MipsOp OR { get; set; }

        public MipsOp XOR { get; set; }

        public MipsOp NOR { get; set; }

        public MipsOp SLT { get; set; }

        public MipsOp SLTU { get; set; }

        public MipsOp DADD { get; set; }

        public MipsOp DADDU { get; set; }

        public MipsOp DSUB { get; set; }

        public MipsOp DSUBU { get; set; }

        public MipsOp TGE { get; set; }

        public MipsOp TGEU { get; set; }

        public MipsOp TLT { get; set; }

        public MipsOp TLTU { get; set; }

        public MipsOp TEQ { get; set; }

        public MipsOp TNE { get; set; }

        public MipsOp DSLL { get; set; }

        public MipsOp DSRL { get; set; }

        public MipsOp DSRA { get; set; }

        public MipsOp DSLL32 { get; set; }

        public MipsOp DSRL32 { get; set; }

        public MipsOp DSRA32 { get; set; }

        public MipsOp BLTZ { get; set; }

        public MipsOp BGEZ { get; set; }

        public MipsOp BLTZL { get; set; }

        public MipsOp BGEZL { get; set; }

        public MipsOp TGEI { get; set; }

        public MipsOp TGEIU { get; set; }

        public MipsOp TLTI { get; set; }

        public MipsOp TLTIU { get; set; }

        public MipsOp TEQI { get; set; }

        public MipsOp TNEI { get; set; }

        public MipsOp BLTZAL { get; set; }

        public MipsOp BGEZAL { get; set; }

        public MipsOp BLTZALL { get; set; }

        public MipsOp BGEZALL { get; set; }

        public MipsOp MCF0 { get; set; }

        public MipsOp DMFC0 { get; set; }

        public MipsOp CFC0 { get; set; }

        public MipsOp MTC0 { get; set; }

        public MipsOp DMTC0 { get; set; }

        public MipsOp CTC0 { get; set; }

        public MipsOp TLBR { get; set; }

        public MipsOp TLBWI { get; set; }

        public MipsOp TLBWR { get; set; }

        public MipsOp TLBP { get; set; }

        public MipsOp ERET { get; set; }

        public MipsOp MFC1 { get; set; }

        public MipsOp DMFC1 { get; set; }

        public MipsOp CFC1 { get; set; }

        public MipsOp MTC1 { get; set; }

        public MipsOp DMTC1 { get; set; }

        public MipsOp CTC1 { get; set; }

        public MipsOp BC1F { get; set; }

        public MipsOp BC1T { get; set; }

        public MipsOp BC1FL { get; set; }

        public MipsOp BC1TL { get; set; }

        public MipsOp FPU_ADD { get; set; }

        public MipsOp FPU_SUB { get; set; }

        public MipsOp FPU_MUL { get; set; }

        public MipsOp FPU_DIV { get; set; }

        public MipsOp FPU_SQRT { get; set; }

        public MipsOp FPU_ABS { get; set; }

        public MipsOp FPU_MOV { get; set; }

        public MipsOp FPU_NEG { get; set; }

        public MipsOp FPU_ROUND_L { get; set; }

        public MipsOp FPU_TRUNC_L { get; set; }

        public MipsOp FPU_CEIL_L { get; set; }

        public MipsOp FPU_FLOOR_L { get; set; }

        public MipsOp FPU_ROUND_W { get; set; }

        public MipsOp FPU_TRUNC_W { get; set; }

        public MipsOp FPU_CEIL_W { get; set; }

        public MipsOp FPU_FLOOR_W { get; set; }

        public MipsOp FPU_CVT_D { get; set; }

        public MipsOp FPU_CVT_W { get; set; }

        public MipsOp FPU_CVT_L { get; set; }

        public MipsOp FPU_C_F { get; set; }

        public MipsOp FPU_C_UN { get; set; }

        public MipsOp FPU_C_EQ { get; set; }

        public MipsOp FPU_C_UEQ { get; set; }

        public MipsOp FPU_C_OLT { get; set; }

        public MipsOp FPU_C_ULT { get; set; }

        public MipsOp FPU_C_OLE { get; set; }

        public MipsOp FPU_C_ULE { get; set; }

        public MipsOp FPU_C_SF { get; set; }

        public MipsOp FPU_C_NGLE { get; set; }

        public MipsOp FPU_C_SEQ { get; set; }

        public MipsOp FPU_C_NGL { get; set; }

        public MipsOp FPU_C_LT { get; set; }

        public MipsOp FPU_C_NGE { get; set; }

        public MipsOp FPU_C_LE { get; set; }

        public MipsOp LHU { get; set; }

        public MipsOp MULTU { get; set; }

        public MipsOp FPU_CVT_S { get; set; }

        public MipsOp FPU_C_NGT { get; set; }
    }
}