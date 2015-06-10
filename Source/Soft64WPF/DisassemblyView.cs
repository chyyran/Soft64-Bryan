using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Soft64WPF
{
    public class DisassemblyView : Control
    {
        private StackPanel m_LineRootPanel;
        private ObservableCollection<DiassemblyLine> m_LineCollection;
    }

    public sealed class DiassemblyLine
    {

    }
}
