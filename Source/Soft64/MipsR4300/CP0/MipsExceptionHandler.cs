using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.CP0
{
    public sealed class MipsExceptionHandler
    {
        private CP0Registers m_Regs;

        public MipsExceptionHandler(CP0Registers regs)
        {
            m_Regs = regs;
        }


    }
}
