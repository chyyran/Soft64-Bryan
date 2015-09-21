using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* MIPS Info
 * Opcode execution = 8 PC cycles
 * 1 PC Cycle = 2 master cycles
 * Opcod execution = 4 master cycles
 * Pipeline Size = 8 stages, so 8 level deep
 */

namespace Soft64.MipsR4300
{
    public class CoreClock
    {
        private Int64 m_CoreCycles;

        public CoreClock()
        {
            m_CoreCycles = 7;
        }

        public void Advance(Int32 cycles)
        {
            m_CoreCycles += cycles;
        }

        public void AdvanceInstruction()
        {
            m_CoreCycles += 1;
        }

        public virtual void BranchDelay()
        {
            m_CoreCycles += 3;
        }

        public virtual void LoadDelay()
        {
            m_CoreCycles += 2;
        }

        public Int32 DelaySlotCycle { get; set; }

        public Int64 Cycles
        {
            get { return m_CoreCycles; }
        }
    }
}
