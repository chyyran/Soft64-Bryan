using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Soft64.Debugging;

namespace Soft64.Engines
{
    public abstract class EmulatorEngine : ILifetimeTrackable
    {
        private LifetimeState m_LifeState = LifetimeState.Created;
        private CoreTaskScheduler m_CoreScheduler;
        private CancellationTokenSource m_TokenSource;
        protected List<Task> m_TaskList = new List<Task>();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<LifeStateChangedArgs> LifetimeStateChanged;

        protected EmulatorEngine()
        {
            m_TokenSource = new CancellationTokenSource();
            m_CoreScheduler = new SingleCoreScheduler();
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

                        case DebuggerEventType.Pause:
                            {
                                PauseThreads();
                                break;
                            };

                        case DebuggerEventType.Break: /* high level CPU break to pause whole emulator */
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

            m_LifeState = LifetimeState.Initialized;
        }

        protected abstract void StartTasks(CancellationToken token, TaskFactory factory);

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

            StartTasks(m_TokenSource.Token, factory);
            m_CoreScheduler.RunThreads();

            OnLifetimeStateChange(m_LifeState, LifetimeState.Running);
            m_LifeState = LifetimeState.Running;
        }

        public void Stop()
        {
            if (m_CoreScheduler != null)
            {
                m_TokenSource.Cancel(false);
            }

            OnLifetimeStateChange(m_LifeState, LifetimeState.Stopped);
            m_LifeState = LifetimeState.Stopped;
        }

        public void PauseThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.PauseThreads();
            }
        }

        public void ResumeThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.ResumeThreads();
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

        public Boolean IsPaused
        {
            get { return m_CoreScheduler.IsPaused; }
        }
    }
}
