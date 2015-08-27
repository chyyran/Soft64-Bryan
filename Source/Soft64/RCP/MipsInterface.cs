using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.RCP
{
    public sealed class MipsInterface : RegisterMappedMemoryStream
    {
        public MipsInterface() : base(0xFFFFF)
        {
            
        }
    }
}
