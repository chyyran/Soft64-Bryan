using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public sealed class SimpleEngine : EmulatorEngine
    {
        public SimpleEngine() : base()
        {
            /* Uses just a single threaded scheduler */
            SetCoreScheduler(new SingleCoreScheduler());
        }

        protected override void StartTasks(System.Threading.CancellationToken token, TaskFactory factory, Action pauseWaitAction)
        {
            /* Create a single loop on the thread */

            factory.StartNew(() =>
            {
                while (true)
                {
                    /* This pause event comes from the core scheduler to pause this task when enabled */
                    pauseWaitAction();

                    /* Execute a step in the CPU */
                    Machine.Current.CPU.StepOnce();
                }

            }, token);
        }
    }
}
