using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64;
using Soft64.MipsR4300.CP0;
using Soft64.MipsR4300.Debugging;
using Soft64.Toolkits.WPF;

namespace Soft64Binding.WPF
{
    public class CpuViewModel : MachineComponentViewModel
    {
        public CpuViewModel(MachineViewModel machineViewModel) : base(machineViewModel)
        {
            var cpu = machineViewModel.TargetMachine.CPU;

            VirtualMemory = StreamViewModel.NewModelFromStream(cpu.VirtualMemoryStream);
            DebugVirtualMemory = StreamViewModel.NewModelFromStream(new VMemViewStream());
            TlbCache = new TlbCacheViewModel(machineViewModel);
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
    }
}
