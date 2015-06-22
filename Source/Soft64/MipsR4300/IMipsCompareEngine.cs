using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public interface IMipsCompareEngine
    {
        void Init();

        void Run();

        Boolean CompareState(ExecutionState state);

        void Release();
    }
}
