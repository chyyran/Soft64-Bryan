using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Soft64.Debugging;
using Soft64.MipsR4300;

namespace Soft64.Engines
{
    public enum EngineStatus
    {
        Started,
        Stopped,
        Running,
        Paused,
        Resumed,
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

    public abstract class EmulatorEngine
    {
        private CoreTaskScheduler m_CoreScheduler;
        private CancellationTokenSource m_TokenSource;
        protected List<Task> m_TaskList = new List<Task>();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private EngineStatus m_Status = EngineStatus.Stopped;
        private Boolean m_SingleStep = false;
        private EventWaitHandle m_SingleStepWaitEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        internal event EventHandler<EngineStatusChangedArgs> EngineStatusChanged;

        internal event EventHandler BeginTrigger;
        internal event EventHandler EndTrigger;

        protected EmulatorEngine()
        {
            m_TokenSource = new CancellationTokenSource();
        }

        protected void End()
        {
            if (m_SingleStep)
            {
                m_SingleStep = false;
                PauseThreads();
                m_SingleStepWaitEvent.Set();
            }

            var e = BeginTrigger;

            if (e != null)
            {
                e(this, new EventArgs());
            }
        }

        protected void Begin()
        {
            /* If cancellation invoked, then throw exception */
            m_TokenSource.Token.ThrowIfCancellationRequested();

            /* This pause event comes from the core scheduler to pause this task when enabled */
            m_CoreScheduler.PauseWait();

            var e = BeginTrigger;

            if (e != null)
            {
                e(this, new EventArgs());
            }
        }

        protected abstract void StartTasks(TaskFactory factory, CancellationToken token);

        public void SetCoreScheduler(CoreTaskScheduler scheduler)
        {
            if (m_Status == EngineStatus.Stopped)
                m_CoreScheduler = scheduler;
        }

        public void Start()
        {
            logger.Trace("Starting emulator engine");

            TaskFactory factory = new TaskFactory(m_CoreScheduler);

            StartTasks(factory, m_TokenSource.Token);
            m_CoreScheduler.RunThreads();

            OnStatusChange(m_Status, EngineStatus.Started);
            m_Status = EngineStatus.Started;
        }

        public void Stop()
        {
            logger.Trace("Stopped emulator engine");

            if (m_CoreScheduler != null)
            {
                m_TokenSource.Cancel(false);
            }

            OnStatusChange(m_Status, EngineStatus.Stopped);
            m_Status = EngineStatus.Stopped;
        }

        public void PauseThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.PauseThreads();

                /* Use an async task to keep event handlers from deadlocking this method */
                Task.Factory.StartNew(() =>
                {
                    OnStatusChange(m_Status, EngineStatus.Paused);
                    m_Status = EngineStatus.Paused;
                }, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default);
                
                logger.Trace("Execution paused");
            }
        }

        public void ResumeThreads()
        {
            if (m_CoreScheduler != null)
            {
                m_CoreScheduler.ResumeThreads();

                OnStatusChange(m_Status, EngineStatus.Started);
                m_Status = EngineStatus.Started;

                logger.Trace("Execution resumed");
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public CoreTaskScheduler CurrentScheduler
        {
            get { return m_CoreScheduler; }
        }

        protected virtual void OnStatusChange(EngineStatus oldStatus, EngineStatus newStatus)
        {
            var e = EngineStatusChanged;

            if (e != null)
            {
                EngineStatusChanged(this, new EngineStatusChangedArgs(oldStatus, newStatus));
            }
        }

        public EngineStatus Status
        {
            get { return m_Status; }
        }
    }
}