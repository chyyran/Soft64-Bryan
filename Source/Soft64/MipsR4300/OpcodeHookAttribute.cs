using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public sealed class OpcodeHookAttribute : Attribute
    {
        public String OpcodeName { get; private set; }

        public OpcodeHookAttribute(String name)
        {
            OpcodeName = name;
        }
    }
}
