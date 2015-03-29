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

        }

        protected override void StartTasks(System.Threading.CancellationToken token, TaskFactory factory)
        {
            factory.StartNew(Machine.Current.CPU.Run);
        }
    }
}
