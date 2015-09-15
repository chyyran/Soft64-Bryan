using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public enum InterruptKind : int
    {
        None = 0x000,
        VideoInterface = 0x001,
        Compare = 0x002,
        Check = 0x004,
        SerialInterface = 0x008,
        PherpherialInterface = 0x010,
        Special = 0x020,
        AudioInterface = 0x040,
        SignalProcessor = 0x080,
        DisplayProcessor = 0x100,
        Hardware2 = 0x200,
        NonMaskable = 0x400
    }

    internal struct _InterruptEvent
    {
        public UInt64 Count;
        public InterruptKind Kind;
    }

    public class InterruptScheduler
    {
        private Queue<_InterruptEvent> m_InterruptQueue;
        private Boolean m_SpecialFinished = false;
        private ExecutionState m_MipsState;
        private UInt64 m_NextInterrupt;

        public InterruptScheduler()
        {
            m_InterruptQueue = new Queue<_InterruptEvent>();
        }

        //public void AddInterruptEvent(InterruptKind kind, UInt64 count)
        //{
        //    if (m_MipsState.CP0Regs.Count > 0x80000000)
        //        m_SpecialFinished = false;

        //    /* TODO: Need hack-fix here ? */

        //    _InterruptEvent iEvent = new _InterruptEvent();
        //    iEvent.Count = count;
        //    iEvent.Kind = kind;

        //    if (m_InterruptQueue.Count < 1)
        //    {
        //        /* First event in the queue */
        //        m_InterruptQueue.Enqueue(iEvent);
        //        m_NextInterrupt = count;
        //    }
        //    //else if ()
        //}

        //private Boolean IsBeforeEvent(UInt64 eventCount, _InterruptEvent newEvent)
        //{
        //    if ((eventCount - m_MipsState.CP0Regs.Count) < 0x80000000)
        //    {

        //    }
        //}
    }
}
