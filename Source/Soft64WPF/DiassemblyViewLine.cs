using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soft64.MipsR4300;

namespace Soft64WPF
{
    public class DiassemblyViewLine : DependencyObject
    {
        public static readonly DependencyProperty LoadedInstructionProperty =
            DependencyProperty.Register("LoadedInstruction", typeof(MipsInstruction), typeof(DiassemblyViewLine),
            new PropertyMetadata());

        public MipsInstruction LoadedInstruction
        {
            get { return (MipsInstruction)GetValue(LoadedInstructionProperty); }
            set { SetValue(LoadedInstructionProperty, value); }
        }

        public String DisasmText
        {
            get { return LoadedInstruction.ToString(); }
        }

        public String AddressText
        {
            get { return LoadedInstruction.PC.ToString("X8"); }
        }
    }
}
