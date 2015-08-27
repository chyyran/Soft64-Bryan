using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Soft64.MipsR4300.Debugging;

namespace Soft64WPF.Converters
{
    public sealed class BreakpointToVisibilityConverter : IValueConverter
    {
        private MipsDebugger m_Debugger;

        public BreakpointToVisibilityConverter(MipsDebugger debugger)
        {
            m_Debugger = debugger;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Int64 address = (Int64)value;
            return m_Debugger.ContainsBreakpoint(address) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
