/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;

namespace Soft64.MipsR4300
{
    public abstract class ExecutionEngine : IDisposable
    {
        private MipsR4300Core m_MipsParent; /* Parent MIPS VR4300 of this engine */
        private Boolean m_Disposed;              /* User disposed flag */
        private Boolean m_Faulted;               /* Engine fault flag */

        public Boolean BranchDelaySlot { get; protected set; }

        /* Engine events */

        public event EventHandler Break;         /* When the CPU has triggered a software break */

        public event EventHandler Stop;          /* When the CPU has been stopped */

        protected ExecutionEngine()
        {
        }

        internal void SetParent(MipsR4300Core currentProcessor)
        {
            m_MipsParent = currentProcessor;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Shutdown()
        {
        }

        public abstract void Step();

        protected virtual void OnBreak()
        {
            EventHandler e = Break;

            if (e != null)
                e(this, new EventArgs());
        }

        protected virtual void OnStop()
        {
            EventHandler e = Stop;

            if (e != null)
                e(this, new EventArgs());
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                }

                m_Disposed = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Boolean HasFaulted
        {
            get { return m_Faulted; }
            protected set { m_Faulted = value; }
        }

        protected MipsR4300Core ParentMips
        {
            get { return m_MipsParent; }
        }
    }
}