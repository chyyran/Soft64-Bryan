using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Soft64;

namespace Soft64WPF.Windows
{
    /// <summary>
    /// Interaction logic for ResourceDebugger.xaml
    /// </summary>
    public partial class ResourceDebugger : Window
    {
        public ResourceDebugger()
        {
            InitializeComponent();
            Machine.Current.DeviceCPU.DebugMemoryAccess = true;
            ((Paragraph)xaml_ResLog.Document.Blocks.FirstBlock).LineHeight = 3;
        }

        private void xaml_CheckBoxCPUMem_Checked(object sender, RoutedEventArgs e)
        {
            if (xaml_CheckBoxCPUMem.IsChecked == true)
            {
                Machine.Current.DeviceCPU.ResourceMonitor.CPUMemoryRead += CPUMemoryCallbackRead;
                Machine.Current.DeviceCPU.ResourceMonitor.CPUMemoryWrite += CPUMemoryCallbackWrite;
            }
            else
            {
                Machine.Current.DeviceCPU.ResourceMonitor.CPUMemoryRead -= CPUMemoryCallbackRead;
                Machine.Current.DeviceCPU.ResourceMonitor.CPUMemoryWrite -= CPUMemoryCallbackWrite;
            }
        }

        private void CPUMemoryCallbackRead(N64MemRegions region, Int64 address)
        {
            Dispatcher.InvokeAsync(() =>
            {
                xaml_ResLog.AppendText(String.Format("CPU VMemory Read Access @ {0:X8} [{1}]\n", address, region.ToString()));
            });
        }

        private void CPUMemoryCallbackWrite(N64MemRegions region, Int64 address)
        {
            Dispatcher.InvokeAsync(() =>
            {
                xaml_ResLog.AppendText(String.Format("CPU VMemory Write Access @ {0:X8} [{1}]\n", address, region.ToString()));
            });
        }
    }
}
