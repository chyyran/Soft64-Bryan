using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public static class InterpreterExtensions
    {
        public static Boolean IsSigned32(this UInt64 value)
        {
            UInt32 val = (UInt32)(value >> 32);
            return val == 0xFFFFFFFF || val == 0x00000000;
        }
    }
}
