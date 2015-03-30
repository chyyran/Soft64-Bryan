using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public abstract class CoreTaskScheduler : TaskScheduler
    {
        private List<Task> m_ScheduledTasks;
        private List<Thread> m_ThreadList;
        private EventWaitHandle m_PauseEvent;

        protected CoreTaskScheduler()
        {
            m_ScheduledTasks = new List<Task>();
            m_ThreadList = new List<Thread>();
            m_PauseEvent = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        public void CleanThreads()
        {
            foreach (var thread in m_ThreadList)
            {
                if (thread.IsAlive && thread.ThreadState == ThreadState.Running)
                {
                    /* Try to kill off the threads still running with a dirty hack */
                    thread.Abort();
                    m_ThreadList.Remove(thread);
                }
            }

            GC.Collect();
        }

        public void PauseThreads()
        {
            lock (m_PauseEvent)
            {
                m_PauseEvent.Reset();
            }
        }

        public void ResumeThreads()
        {
            lock (m_PauseEvent)
            {
                m_PauseEvent.Set();
            }
        }

        public void RunThreads()
        {
            PauseThreads();

            foreach (var task in m_ScheduledTasks)
            {
                var thread = GetTaskThread(task, m_PauseEvent);

                if (thread != null && !thread.IsAlive && thread.ThreadState != ThreadState.Running)
                {
                    thread.IsBackground = true;
                    thread.Start();
                    m_ThreadList.Add(thread);
                }
                
            }

            ResumeThreads();
        }

        public IEnumerable<Thread> GetThreads()
        {
            return m_ThreadList.AsReadOnly();
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return m_ScheduledTasks;
        }

        protected override void QueueTask(Task task)
        {
            m_ScheduledTasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new NotImplementedException();
        }

        protected abstract Thread GetTaskThread(Task task, EventWaitHandle pauseEvent);
    }
}
