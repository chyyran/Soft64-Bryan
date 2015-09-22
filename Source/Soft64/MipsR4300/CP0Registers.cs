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

namespace Soft64.MipsR4300
{
    public enum CP0RegName : int
    {
        /// <summary>
        /// Invalid register name
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// Programmable pointer into TLB array
        /// </summary>
        Index,

        /// <summary>
        /// Pseudorandom pointer into TLB array (read only)
        /// </summary>
        Random,

        /// <summary>
        /// Low half of TLB entry for even virtual address (VPN)
        /// </summary>
        EntryLo0,

        /// <summary>
        /// Low half of TLB entry for odd virtual address (VPN)
        /// </summary>
        EntryLo1,

        /// <summary>
        /// Pointer to kernel virtual page table entry (PTE) in 32-bit addressing mode
        /// </summary>
        Context,

        /// <summary>
        /// TLB Page Mask
        /// </summary>
        PageMask,

        /// <summary>
        /// Number of wired TLB entries
        /// </summary>
        Wired,

        /// <summary>
        /// Bad virtual address
        /// </summary>
        BadVAddr = 8,

        /// <summary>
        /// Timer Count
        /// </summary>
        Count,

        /// <summary>
        /// High half of TLB entry
        /// </summary>
        EntryHi,

        /// <summary>
        /// Timer Compare
        /// </summary>
        Compare,

        /// <summary>
        /// Status register
        /// </summary>
        SR,

        /// <summary>
        /// Cause of last exception
        /// </summary>
        Cause,

        /// <summary>
        /// Exception Program Counter
        /// </summary>
        EPC,

        /// <summary>
        /// Processor Revision Identifier
        /// </summary>
        PRId,

        /// <summary>
        /// Configuration register
        /// </summary>
        Config,

        /// <summary>
        /// Load Linked Address
        /// </summary>
        LLAddr,

        /// <summary>
        /// Memory reference trap address low bits
        /// </summary>
        WatchLo,

        /// <summary>
        /// Memory reference trap address high bits
        /// </summary>
        WatchHi,

        /// <summary>
        /// Pointer to kernel virtual PTE table in 64-bit addressing mode
        /// </summary>
        XContext,

        /// <summary>
        /// Secondary-cache error checking and correcting (ECC) and Primary parity
        /// </summary>
        ECC = 26,

        /// <summary>
        /// Cache Error and Status register
        /// </summary>
        CacheErr,

        /// <summary>
        /// Cache Tag register
        /// </summary>
        TagLo,

        /// <summary>
        /// Cache Tag register
        /// </summary>
        TagHi,

        /// <summary>
        /// Error Exception Program Counter
        /// </summary>
        ErrorEPC,
    };

    [Serializable]
    public sealed class CP0Registers
    {
        private UInt64[] m_Regs;
        private StatusRegister m_SR;
        private CauseRegister m_CauseReg;
        private UInt32 m_Count;
        private UInt32 m_Compare;
        private UInt64 m_BadVAddress;
        private UInt64 m_EntryHi;
        private UInt64 m_EntryLo0;
        private UInt64 m_EntryLo1;
        private UInt64 m_Index;
        private UInt64 m_PageMask;
        private UInt64 m_Random;
        private UInt64 m_Wired;
        private UInt64 m_Context;
        private UInt64 m_EPC;
        private UInt64 m_Prid;
        private UInt64 m_Config;
        private UInt64 m_LLAddr;
        private UInt64 m_WatchLo;
        private UInt64 m_WatchHi;
        private UInt64 m_XContext;
        private UInt64 m_ECC;
        private UInt64 m_CacheErr;
        private UInt64 m_TagLo;
        private UInt64 m_TagHi;
        private UInt64 m_ErrorEPC;

        public CP0Registers()
        {
            m_Regs = new UInt64[32];
            m_SR = new StatusRegister();
            m_CauseReg = new CauseRegister();
        }

        public UInt64 this[Int32 index]
        {
            get
            {
                switch (index)
                {
                    default: return 0;
                    case 0: return m_Index;
                    case 1: return m_Random;
                    case 2: return m_EntryLo0;
                    case 3: return m_EntryLo1;
                    case 4: return m_Context;
                    case 5: return m_PageMask;
                    case 6: return m_Wired;
                    case 8: return m_BadVAddress;
                    case 9: return m_Count;
                    case 10: return m_EntryHi;
                    case 11: return m_Compare;
                    case 12: return m_SR.RegisterValue64;
                    case 13: return m_CauseReg.RegisterValue64;
                    case 14: return m_EPC;
                    case 15: return m_Prid;
                    case 16: return m_Config;
                    case 17: return m_LLAddr;
                    case 18: return m_WatchLo;
                    case 19: return m_WatchHi;
                    case 20: return m_XContext;
                    case 26: return m_ECC;
                    case 27: return m_CacheErr;
                    case 28: return m_TagLo;
                    case 29: return m_TagHi;
                    case 30: return m_ErrorEPC;
                }
            }

            set
            {
                switch (index)
                {
                    default: return;
                    case 0: m_Index = value; break;
                    case 2: m_EntryLo0 = value; break;
                    case 3: m_EntryLo1 = value; break;
                    case 4: m_Context = value; break;
                    case 5: m_PageMask = value; break;
                    case 9: m_Count = (UInt32)value; break;
                    case 10: m_EntryHi = value; break;
                    case 11: m_Compare = (UInt32)value; break;
                    case 12: m_SR.RegisterValue64 = value; break;
                    case 13: m_CauseReg.RegisterValue64 = value; break;
                    case 14: m_EPC = value; break;
                    case 15: m_Prid = value; break;
                    case 16: m_Config = value; break;
                    case 17: m_LLAddr = value; break;
                    case 18: m_WatchLo = value; break;
                    case 19: m_WatchHi = value; break;
                    case 20: m_XContext = value; break;
                    case 26: m_ECC = value; break;
                    case 27: m_CacheErr = value; break;
                    case 28: m_TagLo = value; break;
                    case 29: m_TagHi = value; break;
                    case 30: m_ErrorEPC = value; break;
                }
            }
        }

        public StatusRegister StatusReg
        {
            get { return m_SR; }
        }

        public CauseRegister CauseReg
        {
            get { return m_CauseReg; }
        }

        public UInt64 Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public UInt64 Random
        {
            get { return m_Random; }
            set { m_Random = value; }
        }

        public UInt64 EntryLo0
        {
            get { return m_EntryLo0; }
            set { m_EntryLo0 = value; }
        }

        public UInt64 EntryLo1
        {
            get { return m_EntryLo1; }
            set { m_EntryLo1 = value; }
        }

        public UInt64 Context
        {
            get { return m_Context; }
            set { m_Context = value; }
        }

        public UInt64 PageMask
        {
            get { return m_PageMask; }
            set { m_PageMask = value; }
        }

        public UInt64 Wired
        {
            get { return m_Wired; }
            set { m_Wired = value; }
        }

        public UInt64 BadVAddr
        {
            get { return m_BadVAddress; }
            set { m_BadVAddress = value; }
        }

        public UInt32 Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        public UInt64 EntryHi
        {
            get { return m_EntryHi; }
            set { m_EntryHi = value; }
        }

        public UInt32 Compare
        {
            get { return m_Compare; }
            set { m_Compare = value; m_Count = 0; }
        }

        public UInt64 Cause
        {
            get { return m_CauseReg.RegisterValue64; }
            set { m_CauseReg.RegisterValue64 = value; }
        }

        public UInt64 Status
        {
            get { return m_SR.RegisterValue64; }
            set { m_SR.RegisterValue64 = value; }
        }

        public UInt64 EPC
        {
            get { return m_EPC; }
            set { m_EPC = value; }
        }

        public UInt64 PRId
        {
            get { return m_Prid; }
            set { m_Prid = value; }
        }

        public UInt64 Config
        {
            get { return m_Config; }
            set { m_Config = value; }
        }

        public UInt64 LLAddr
        {
            get { return m_LLAddr; }
            set { m_LLAddr = value; }
        }

        public UInt64 WatchLo
        {
            get { return m_WatchLo; }
            set { m_WatchLo = value; }
        }

        public UInt64 WatchHi
        {
            get { return m_WatchHi; }
            set { m_WatchHi = value; }
        }

        public UInt64 XContext
        {
            get { return m_XContext; }
            set { m_XContext = value; }
        }

        public UInt64 ECC
        {
            get { return m_ECC; }
            set { m_ECC = value; }
        }

        public UInt64 CacheErr
        {
            get { return m_CacheErr; }
            set { m_CacheErr = value; }
        }

        public UInt64 TagLo
        {
            get { return m_TagLo; }
            set { m_TagLo = value; }
        }

        public UInt64 TagHi
        {
            get { return m_TagHi; }
            set { m_TagHi = value; }
        }

        public UInt64 ErrorEPC
        {
            get { return m_ErrorEPC; }
            set { m_ErrorEPC = value; }
        }
    }
}