using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300.CP0;
using Soft64.MipsR4300.CP1;

namespace Soft64.MipsR4300
{
    public class MipsSnapshot
    {
        private Int64 m_PC;
        private Boolean m_Mode;
        private GPRRegisters m_GPR;
        private CP0Registers m_CP0;
        private FprRegisters m_CP1;
        private UInt64 m_Hi;
        private UInt64 m_Lo;
        private Boolean m_LLbit;
        private UInt32 m_FCR0;
        private UInt32 m_FCR31;

        public MipsSnapshot()
        {
            m_GPR = new GPRRegisters();
            m_CP0 = new CP0Registers();
            m_CP1 = new FprRegisters();
        }

        public Int64 PC
        {
            get { return m_PC; }
            set { m_PC = value; }
        }

        public Boolean WordMode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        public GPRRegisters GPR
        {
            get
            {
                return m_GPR;
            }
        }

        public CP0Registers CP0
        {
            get
            {
                return m_CP0;
            }
        }

        public FprRegisters CP1
        {
            get
            {
                return m_CP1;
            }
        }

        public UInt64 Hi
        {
            get { return m_Hi; }
            set { m_Hi = value; }
        }

        public UInt64 Lo
        {
            get { return m_Lo; }
            set { m_Lo = value; }
        }

        public Boolean LLbit
        {
            get { return m_LLbit; }
            set { m_LLbit = value; }
        }

        public UInt32 FCR0
        {
            get { return m_FCR0; }
            set { m_FCR0 = value; }
        }

        public UInt32 FCR31
        {
            get { return m_FCR31; }
            set { m_FCR31 = value; }
        }
    }
}
