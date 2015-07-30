using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging
{
    public delegate void MemoryExecute(N64MemRegions region, Int64 address);

    public sealed class IOMonitor
    {
        public event MemoryExecute CPUMemoryRead;
        public event MemoryExecute CPUMemoryWrite;

        internal IOMonitor()
        {

        }

        internal void CPUMemRead(Int64 address)
        {

        }

        internal void CPUMemWrite(Int64 address)
        {

        }

        public Boolean Enabled { get; set; }
    }
}
