using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.Dog
{
    public sealed class MemorySniffer
    {
        private static MemorySniffer s_CurrentSniffer;


        public MemorySniffer()
        {
            if (s_CurrentSniffer == null)
            {
                s_CurrentSniffer = this;
            }
            else
            {
                throw new InvalidOperationException("MemorySniffer is a singleton class");
            }
        }

        public void Sniff()
        {

        }

        public static MemorySniffer Current
        {
            get { return s_CurrentSniffer; }
        }
    }
}
