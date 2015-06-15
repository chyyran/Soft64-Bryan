using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Soft64.Debugging;

namespace Soft64.Engines
{
    public enum EngineStatus
    {
        Created,
        WaitingForTasks,
        Running,
        Paused,
        Stopped
    }

    public class EngineStatusChangedArgs : EventArgs
    {
        private EngineStatus m_OldStatus;
        private EngineStatus m_NewStatus;

        public EngineStatusChangedArgs(EngineStatus oldStatus, EngineStatus newStatus)
        {
            m_OldStatus = oldStatus;
            m_NewStatus = newStatus;
        }

        public EngineStatus OldStatus
        {
            get { return m_OldStatus; }
        }

        public EngineStatus NewStatus
        {
            get { return m_NewStatus; }
        }
    }

    public abstract class EmulatorEngine : ILifetimeTrackable
    {
        private LifetimeState m_LifeState = LifetimeState.Created;
        private CoreTaskScheduler m_CoreScheduler;
        private CancellationTokenSource m_TokenSource;
        protected List<Task> m_TaskList = new List<Task>();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private EngineStatus m_Status = EngineStatus.Created;

        public event EventHandler<LifeStateChangedArgs> LifetimeStateChanged;

        public event EventHandler<EngineStatusChangedArgs> EngineStatusChanged;

        protected EmulatorEngine()
        {
            m_TokenSource = new CancellationTokenSource();
        }

        private void HookIntoDebugger()
        {
            if (Debugger.Current != null)
            {
                Debugger.Current.RegisterEngineHook((t) =>
                {
                    switch (t)
                    {
                        default: break;

                        case DebuggerEventType.Break: /* high level CPU break to pause whole emulator */
                        case DebuggerEventType.Pause:
                            {
                                PauseThreads();
                                break;
                            };

                        case DebuggerEventType.Resume:
                            {
                                ResumeThreads();
                                break;
                            };

                        case DebuggerEventType.StepOnce:
                            {
                                m_CoreScheduler.ExecuteNext();
                                break;
                            };
                    }
                });
            }
        }

        public virtual void Initialize()
        {
            HookIntoDebugger();

            OnLifetimeStateChange(m_LifeState, LifetimeState.Initialized);
            m_LifeState = LifetimeState.Initialized;

            OnStatusChange(m_Status, EngineStatus.WaitingForTasks);
            m_Status = EngineStatus.WaitingForTasks;
        }

        protected abstract void StartTasks(CancellationToken token, TaskFactory factory, Action pauseWaitAction);

        public void SetCoreScheduler(CoreTaskScheduler scheduler)
        {
            if (m_LifeState < LifetimeState.Initialized)
            {
                m_CoreScheduler = scheduler;
            }
            else
            {
                throw new InvalidOperationException("Cannot set scheduler after initialization state");
            }
        }

        public void Run()
        {
            logger.Trace("Scheduling engine tasks");

            TaskFactory factory = new TaskFactory(m_CoreScheduler);

            StartTasks(m_TokenSource.Token, factory, m_CoreScheduler.PauseWait);
            m_CoreScheduler.RunThreads();

            OnLifetimeStateChange(m_LifeState, LifetimeState.Running);
            m_LifeState = LifetimeState.Running;

            OnStatusChange(m_Status, EngineStatus.Running);
            m_Status = EngineStatus.Running;
        }

        public void Stop()
        {
            if (m_CoreScheduler != null)
            {
                m_TokenSource.Cancel(false);
            }

            OnLifetimeStateChange(m_LifeState, LifetimeState.Stopped);
            m_LifeState = LifetimeState.Stopped;

            OnStatusChange(m_Status, EngineStatus.Stopped);
            m_Status = EngineStatus.Stopped;
        }

        public void PauseThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.PauseThreads();

                OnStatusChange(m_Status, EngineStatus.Paused);
                m_Status = EngineStatus.Paused;
            }
        }

        public void ResumeThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.ResumeThreads();

                OnStatusChange(m_Status, EngineStatus.Running);
                m_Status = EngineStatus.Running;
            }
        }

        public LifetimeState CurrentLifeState
        {
            get { return m_LifeState; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public CoreTaskScheduler CurrentScheduler
        {
            get { return m_CoreScheduler; }
        }

        protected virtual void OnLifetimeStateChange(LifetimeState old, LifetimeState newState)
        {
            var e = LifetimeStateChanged;

            if (e != null)
            {
                e(this, new LifeStateChangedArgs(newState, old));
            }
        }

        protected virtual void OnStatusChange(EngineStatus oldStatus, EngineStatus newStatus)
        {
            var e = EngineStatusChanged;

            if (e != null)
            {
                EngineStatusChanged(this, new EngineStatusChangedArgs(oldStatus, newStatus));
            }
        }

        public Boolean IsPaused
        {
            get { return m_CoreScheduler.IsPaused; }
        }

        public EngineStatus Status
        {
            get { return m_Status; }
        }
    }
}