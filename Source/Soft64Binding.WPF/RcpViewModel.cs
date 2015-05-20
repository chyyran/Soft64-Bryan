using System.Windows;
using Soft64;
using Soft64.Toolkits.WPF;

namespace Soft64Binding.WPF
{
    public sealed class RcpViewModel : DependencyObject
    {
        private MachineViewModel m_MachineModel;

        private static readonly DependencyPropertyKey N64MemoryPropertyKey =
            DependencyProperty.RegisterReadOnly("N64Memory", typeof(StreamViewModel), typeof(RcpViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey SafeN64MemoryPropertyKey =
            DependencyProperty.RegisterReadOnly("SafeN64Memory", typeof(StreamViewModel), typeof(RcpViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty N64MemoryProperty =
            N64MemoryPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SafeN64MemoryProperty =
            SafeN64MemoryPropertyKey.DependencyProperty;

        internal RcpViewModel(MachineViewModel model)
        {
            m_MachineModel = model;
            Machine machine = model.TargetMachine;
            SetValue(SafeN64MemoryPropertyKey, StreamViewModel.NewModelFromStream(machine.DeviceRCP.SafeN64Memory));
            SetValue(N64MemoryPropertyKey, StreamViewModel.NewModelFromStream(machine.DeviceRCP.N64Memory));
        }

        public StreamViewModel N64Memory
        {
            get { return (StreamViewModel)GetValue(N64MemoryProperty); }
        }

        public StreamViewModel SafeN64Memory
        {
            get { return (StreamViewModel)GetValue(SafeN64MemoryProperty); }
        }
    }
}