﻿using System;
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

    public class Debugger : IDisposable
    {
        public event DebuggerEvent DebugBreak;
        public event DebuggerEvent DebugResume;
        public event DebuggerEvent DebugPause;
        public event DebuggerEvent StepOnce;
        public event DebuggerEvent StepIn;
        public event DebuggerEvent StepOut;
        public event DebuggerEvent StepOver;

        private DebuggerEngineEvent m_EngineHook;
        private static Debugger s_CurrentDebugger;
        private Boolean m_Disposed;

        public Debugger()
        {
            if (s_CurrentDebugger != null)
            {
                s_CurrentDebugger.Dispose();
            }

            s_CurrentDebugger = this;
        }

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

        public static Debugger Current
        {
            get { return s_CurrentDebugger; }
        }

        public void Pause()
        {
            OnPause();
        }

        public void Resume()
        {
            OnResume();
        }

        public void Break()
        {
            OnBreak();
        }

        protected virtual void OnBreak()
        {
            OnEvent(DebuggerEventType.Break, DebugBreak);
        }

        protected virtual void OnResume()
        {
            OnEvent(DebuggerEventType.Resume, DebugResume);
        }

        protected virtual void OnPause()
        {
            OnEvent(DebuggerEventType.Pause, DebugPause);
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

        protected void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                m_Disposed = true;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }
}
