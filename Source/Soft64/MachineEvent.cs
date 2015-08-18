using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64
{
    public sealed class MachineEventNotificationArgs : EventArgs
    {
        private MachineEventType m_EventType;

        public MachineEventNotificationArgs(MachineEventType type)
        {
            m_EventType = type;
        }

        public MachineEventType EventType
        {
            get { return m_EventType; }
        }
    }
}
