using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public sealed class SingleThreadTaskScheduler : TaskScheduler
    {
        private Queue<Task> m_EmuTasksQueue = new Queue<Task>();
        private List<Task> m_ScheduledTasks = new List<Task>();
        private Thread m_TaskThread;

        [ThreadStatic]
        private Boolean m_Stop = false;

        public SingleThreadTaskScheduler()
        {
            m_TaskThread = new Thread(TaskThreadStart);
        }

        public void TaskThreadStart(Object obj)
        {
            while (!m_Stop)
            {
                foreach (var task in GetScheduledTasks())
                {
                    TryExecuteTask(task);
                }
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return m_ScheduledTasks;
        }

        protected override void QueueTask(Task task)
        {
            lock (m_EmuTasksQueue)
            {
                m_EmuTasksQueue.Enqueue(task);
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            throw new NotImplementedException();
        }
    }
}
