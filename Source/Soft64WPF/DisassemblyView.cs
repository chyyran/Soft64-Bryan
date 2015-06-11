using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Soft64.MipsR4300;
using Soft64.MipsR4300.Debugging;
using ObservableDisasmLines = System.Collections.ObjectModel.ObservableCollection<Soft64WPF.DiassemblyViewLine>;

namespace Soft64WPF
{
    public partial class DisassemblyView : ContentControl
    {
        private Stream m_VMemory;
        private Int32 m_LineCount;

        static DisassemblyView()
        {

        }

        public DisassemblyView()
        {
            m_VMemory = new VMemViewStream();
        }

        private void ReadDiasm()
        {
            m_VMemory.Position = VMemoryOffset;
            MipsInstruction[] instructions = new MipsInstruction[m_LineCount];
            BinaryReader reader = new BinaryReader(m_VMemory);

            for (Int32 i = 0; i < m_LineCount; i+=4)
            {
                instructions[i] = new MipsInstruction(0, reader.ReadUInt32());
            }


        }


        private static readonly DependencyPropertyKey DisasmLinesPropertyKey =
            DependencyProperty.RegisterReadOnly("DisasmLines", typeof(ObservableDisasmLines), typeof(DisassemblyView),
            new PropertyMetadata(new ObservableDisasmLines()));

        public static readonly DependencyProperty DisasmLinesProperty =
            DisasmLinesPropertyKey.DependencyProperty;

        public ObservableDisasmLines DisasmLines
        {
            get { return (ObservableDisasmLines)GetValue(DisasmLinesProperty); }
            private set { SetValue(DisasmLinesPropertyKey, value); }
        }

        public static readonly DependencyProperty VMemoryOffsetProperty =
            DependencyProperty.Register("VMemoryOffset", typeof(Int64), typeof(DisassemblyView),
            new PropertyMetadata());

        public Int64 VMemoryOffset
        {
            get { return (Int64)GetValue(VMemoryOffsetProperty); }
            set { SetValue(VMemoryOffsetProperty, value); }
        }
    }
}
