using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64.Engines;

namespace Soft64.Debugging
{
    public delegate void DebuggerEvent();

    public delegate void DebuggerEngineEvent(DebuggerEventType type);

    public enum DebuggerEventType
    {
        Break,
        Resume,
        Pause,
        StepOnce,
        StepIn,
        StepOut,
        StepOver
    }

    public class Debugger
    {
        public event DebuggerEvent Break;
        public event DebuggerEvent Resume;
        public event DebuggerEvent Pause;
        public event DebuggerEvent StepOnce;
        public event DebuggerEvent StepIn;
        public event DebuggerEvent StepOut;
        public event DebuggerEvent StepOver;

        private DebuggerEngineEvent m_EngineHook;

        public void RegisterEngineHook(DebuggerEngineEvent hook)
        {
            m_EngineHook = hook;
        }

        private void OnEvent(DebuggerEventType type, DebuggerEvent e)
        {
            if (m_EngineHook != null)
                m_EngineHook(type);

            if (e != null)
            {
                e();
            }
        }

        protected virtual void OnBreak()
        {
            OnEvent(DebuggerEventType.Break, Break);
        }

        protected virtual void OnResume()
        {
            OnEvent(DebuggerEventType.Resume, Resume);
        }

        protected virtual void OnPause()
        {
            OnEvent(DebuggerEventType.Pause, Pause);
        }

        protected virtual void OnStepOnce()
        {
            OnEvent(DebuggerEventType.StepOnce, StepOnce);
        }

        protected virtual void OnStepIn()
        {
            OnEvent(DebuggerEventType.StepIn, StepIn);
        }

        protected virtual void OnStepOut()
        {
            OnEvent(DebuggerEventType.StepOut, StepOut);
        }

        protected virtual void OnStepOver()
        {
            OnEvent(DebuggerEventType.StepOver, StepOver);
        }
    }
}
