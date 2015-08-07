using System.Windows;
using Soft64.MipsR4300.Debugging;
using Soft64.Toolkits.WPF;

namespace Soft64Binding.WPF
{
    public class CpuViewModel : MachineComponentViewModel
    {
        public CpuViewModel(MachineViewModel machineViewModel)
            : base(machineViewModel)
        {
            var cpu = machineViewModel.TargetMachine.DeviceCPU;

            VirtualMemory = StreamViewModel.NewModelFromStream(cpu.VirtualMemoryStream);
            DebugVirtualMemory = StreamViewModel.NewModelFromStream(new VMemViewStream());
            TlbCache = new TlbCacheViewModel(machineViewModel);
            State = new ExecutionStateViewModel(machineViewModel, machineViewModel.TargetMachine.DeviceCPU.State);
            Debugger = new MipsDebuggerViewModel(machineViewModel);
        }

        private static readonly DependencyPropertyKey DebuggerPropertyKey =
            DependencyProperty.RegisterReadOnly("Debugger", typeof(MipsDebuggerViewModel), typeof(CpuViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty DebuggerProperty =
            DebuggerPropertyKey.DependencyProperty;

        public MipsDebuggerViewModel Debugger
        {
            get { return (MipsDebuggerViewModel)GetValue(DebuggerProperty); }
            private set { SetValue(DebuggerPropertyKey, value); }
        }


        private static readonly DependencyPropertyKey VirtualMemoryPropertyKey =
            DependencyProperty.RegisterReadOnly("VirtualMemory", typeof(StreamViewModel), typeof(CpuViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty VirtualMemoryProperty
            = VirtualMemoryPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey DebugVirtualMemoryPropertyKey =
            DependencyProperty.RegisterReadOnly("DebugVirtualMemory", typeof(StreamViewModel), typeof(CpuViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty DebugVirtualMemoryProperty
            = DebugVirtualMemoryPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey TlbCachePropertyKey =
            DependencyProperty.RegisterReadOnly("TlbCache", typeof(TlbCacheViewModel), typeof(CpuViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty TlbCacheProperty =
            TlbCachePropertyKey.DependencyProperty;

        public TlbCacheViewModel TlbCache
        {
            get { return (TlbCacheViewModel)GetValue(TlbCacheProperty); }
            private set { SetValue(TlbCachePropertyKey, value); }
        }

        public StreamViewModel VirtualMemory
        {
            get { return (StreamViewModel)GetValue(VirtualMemoryProperty); }
            private set { SetValue(VirtualMemoryPropertyKey, value); }
        }

        public StreamViewModel DebugVirtualMemory
        {
            get { return (StreamViewModel)GetValue(DebugVirtualMemoryProperty); }
            private set { SetValue(DebugVirtualMemoryPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey StatePropertyKey =
            DependencyProperty.RegisterReadOnly("State", typeof(ExecutionStateViewModel), typeof(CpuViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty StateProperty =
            StatePropertyKey.DependencyProperty;

        public ExecutionStateViewModel State
        {
            get { return (ExecutionStateViewModel)GetValue(StateProperty); }
            private set { SetValue(StatePropertyKey, value); }
        }
    }
}