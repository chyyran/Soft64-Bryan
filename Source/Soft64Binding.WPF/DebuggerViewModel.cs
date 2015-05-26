using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.Debugging;

namespace Soft64Binding.WPF
{
    public class DebuggerViewModel : DependencyObject
    {
        public DebuggerViewModel()
        {
            DebugOnBreak = Debugger.Current.DebugOnBreak;
            DebugOnBreakMode = Debugger.Current.BreakOnBootMode;
        }


        public static readonly DependencyProperty DebugOnBreakProperty =
            DependencyProperty.Register("DebugOnBreak", typeof(Boolean), typeof(DebuggerViewModel),
            new PropertyMetadata(DebugOnBreak_ChangeHandler));

        public Boolean DebugOnBreak
        {
            get { return (Boolean)GetValue(DebugOnBreakProperty); }
            set { SetValue(DebugOnBreakProperty, value); }
        }

        private static void DebugOnBreak_ChangeHandler(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            Debugger.Current.DebugOnBreak =  (Boolean)a.NewValue;
        }

        public static readonly DependencyProperty DebugOnBreakModeProperty =
            DependencyProperty.Register("DebugOnBreakMode", typeof(DebuggerBootEvent), typeof(DebuggerViewModel),
            new PropertyMetadata(DebugOnBreakMode_ChangeHandler));

        public DebuggerBootEvent DebugOnBreakMode
        {
            get { return (DebuggerBootEvent)GetValue(DebugOnBreakModeProperty); }
            set { SetValue(DebugOnBreakModeProperty, value); }
        }

        private static void DebugOnBreakMode_ChangeHandler(DependencyObject o, DependencyPropertyChangedEventArgs a)
        {
            Debugger.Current.BreakOnBootMode = (DebuggerBootEvent)a.NewValue;
        }
    }
}
