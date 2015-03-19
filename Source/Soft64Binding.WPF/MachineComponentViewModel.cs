using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soft64Binding.WPF
{
    public abstract class MachineComponentViewModel : DependencyObject
    {
        private MachineViewModel m_MachineParent;

        internal protected MachineComponentViewModel(MachineViewModel parentMachine)
        {
            m_MachineParent = parentMachine;
        }

        internal protected MachineViewModel ParentMachine
        {
            get { return m_MachineParent; }
        }

        /* TODO: This class will be base model for sub-models of the machine model,
         * Question: is this good choise of name?
         * Idea: MachineComponentViewModel ? */
    }
}
