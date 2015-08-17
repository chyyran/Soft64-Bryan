using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Debugging.Dog
{
    public struct Branch
    {
        public Int64 Offset { get; set; }
        public Int64 Target { get; set; }
    }
}
