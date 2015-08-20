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
using System.Diagnostics;
using System.Text;
using NLog;
using Soft64.IO;
using Soft64.MipsR4300.CP0;

/* Notes: CPU Memory addressing regions
 * ------------------
 * 0x00000000 - 0x7FFFFFFF: TLB Mapped
 * 0x80000000 - 0x9FFFFFFF: Directly mapped to physical memory but uses cache
 * 0xA0000000 - 0xBFFFFFFF: Directly mapped to physical memory but does not use cache
 * 0xC0000000 - 0xDFFFFFFF: TLB Mapped
 * 0xE0000000 - 0xFFFFFFFF: TLB Mapped
 */

namespace Soft64.MipsR4300.IO
{
    /// <summary>
    /// Provides the virtual memory management and CPU addressing logic core for the MIPS R4300I engine.
    /// </summary>
    [Serializable]
    public sealed class VMemStream : UnifiedStream
    {
        private TLBCache m_TLBCache;
        private CP0Registers m_Cp0Regs;
        private StringBuilder m_StrBuilder = new StringBuilder();
        private Boolean m_DebugIO = false;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private ReadOp ReadOperationHandler;
        private WriteOp WriteOperationHandler;
        private MipsR4300Core m_Core;

        delegate Int32 ReadOp(Byte[] buffer, Int32 offset, Int32 count);
        delegate void WriteOp(Byte[] buffer, Int32 offset, Int32 count);


        public VMemStream(MipsR4300Core core)
        {
            m_Core = core;
            m_Cp0Regs = core.State.CP0Regs;
            m_TLBCache = new TLBCache(m_Cp0Regs);
            SetupOperations(false);
        }

        public void SetupOperations(Boolean monitorOps)
        {
            if (monitorOps)
            {
                ReadOperationHandler = (b, o, c) =>
                    {
                        if (m_Core.State.PC != Position && m_Core.State.BranchPC != Position)
                            Machine.Current.DeviceCPU.ResourceMonitor.CPUMemRead(Position);
                        return base.Read(b, o, c);
                    };

                WriteOperationHandler = (b, o, c) =>
                {
                    Machine.Current.DeviceCPU.ResourceMonitor.CPUMemWrite(Position);
                    base.Write(b, o, c);
                };
            }
            else
            {
                ReadOperationHandler = (b, o, c) =>
                {
                    return base.Read(b, o, c);
                };

                WriteOperationHandler = (b, o, c) =>
                {
                    base.Write(b, o, c);
                };
            }
        }

        public void Initialize()
        {
            m_TLBCache.Initialize();
            logger.Trace("TLB Cache Intialized");

            Add(0x00000000, new TLBMapStream(m_TLBCache, 0x80000000));
            Add(0x80000000, new PhysicalMapStream());
            Add(0xA0000000, new PhysicalMapStream());
            Add(0xC0000000, new TLBMapStream(m_TLBCache, 0x20000000));
            Add(0xE0000000, new TLBMapStream(m_TLBCache, 0x20000000));
            logger.Trace("VMap Initialized");
        }

        public override long Length
        {
            get { return 0x100000000; }
        }

        public TLBCache TLB
        {
            get { return m_TLBCache; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                return ReadOperationHandler(buffer, offset, count);
            }
            catch (TLBException e)
            {
                switch (e.ExceptionType)
                {
                    case TLBExceptionType.Mod: Machine.Current.DeviceCPU.State.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbMod; break;
                    default: Machine.Current.DeviceCPU.State.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbLoad; break;
                }

                return count;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                WriteOperationHandler(buffer, offset, count);
            }
            catch (TLBException e)
            {
                switch (e.ExceptionType)
                {
                    case TLBExceptionType.Mod: Machine.Current.DeviceCPU.State.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbMod; break;
                    default: Machine.Current.DeviceCPU.State.CP0Regs.CauseReg.ExceptionType = ExceptionCode.TlbStore; break;
                }
            }
        }


        public bool DebugMode
        {
            get { return m_DebugIO; }
            set { m_DebugIO = value; }
        }
    }
}