using System.Windows;
using Soft64;

namespace Soft64Binding.WPF
{
    public sealed class RcpViewModel : MachineComponentViewModel
    {
        private MachineViewModel m_MachineModel;

        internal RcpViewModel(MachineViewModel model) :
            base(model)
        {
            m_MachineModel = model;
            Machine machine = model.TargetMachine; ;
        }
    }
}