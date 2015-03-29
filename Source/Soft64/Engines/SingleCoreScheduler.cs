using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public sealed class SingleCoreScheduler : CoreTaskScheduler
    {
        private Thread m_SingleThread;
        private Action m_CallChain;

        public SingleCoreScheduler() : base()
        {

        }

        protected override System.Threading.Thread GetTaskThread(Task task, System.Threading.EventWaitHandle pauseEvent)
        {
            Action taskAction = new Action(() => this.TryExecuteTask(task));

            if (m_CallChain == null)
            {
                m_CallChain = taskAction;
            }
            else
            {
                MulticastDelegate.Combine(m_CallChain, taskAction);
            }

            if (m_SingleThread == null)
            {
                m_SingleThread = new Thread((o) => {
                    while (true)
                    {
                        /* If the lock is on the event, wait until its released */
                        if (Monitor.IsEntered(pauseEvent))
                            Monitor.Wait(pauseEvent);

                        /* Thread will be paused here until notified */
                        pauseEvent.WaitOne();

                        /* Call the chained tasks */
                        m_CallChain();
                    }
                });
            }

            return m_SingleThread;
        }
    }
}
