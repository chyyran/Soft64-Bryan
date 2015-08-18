using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Soft64.Engines
{
    public sealed class SimpleEngine : EmulatorEngine
    {
        public SimpleEngine()
            : base()
        {
            /* Uses just a single threaded scheduler */
            SetCoreScheduler(new SingleCoreScheduler());
        }

        protected override void StartTasks(TaskFactory factory, CancellationToken token)
        {
            /* Create a single loop on the thread */
            Action singleLoop = () =>
            {
                Machine.Current.Boot();

                OnStatusChange(this.Status, EngineStatus.Running);

                while (true)
                {
                    Begin();

                    /* Execute a step in the CPU */
                    Machine.Current.DeviceCPU.StepOnce();

                    End();
                }
            };

            RuntimeHelpers.PrepareDelegate(singleLoop);

            factory.StartNew(singleLoop, token);
        }
    }
}