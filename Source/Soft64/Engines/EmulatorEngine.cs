using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

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

        public virtual void Initialize()
        {
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

        public LifetimeState CurrentRuntimeState
        {
            get { return m_LifeState; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLifetimeStateChange(LifetimeState old, LifetimeState newState)
        {
            var e = LifetimeStateChanged;

            if (e != null)
            {
                e(this, new LifeStateChangedArgs(newState, old));
            }
        }
    }
}
