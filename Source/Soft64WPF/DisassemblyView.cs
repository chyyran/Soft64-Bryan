using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Soft64;
using Soft64.IO;
using Soft64.MipsR4300;
using Soft64.MipsR4300.Debugging;
using ObservableDisasmLines = System.Collections.ObjectModel.ObservableCollection<Soft64WPF.DiassemblyViewLine>;

namespace Soft64WPF
{
    public partial class DisassemblyView : ContentControl
    {
        private Stream m_VMemory;
        private Int32 m_LineCount;
        private static event EventHandler fontChanged;

        static DisassemblyView()
        {
            FontTools.RegisterFontChangeEvents(typeof(DisassemblyView), fontChanged);
        }

        public DisassemblyView()
        {
            if (Machine.Current != null)
            {
                m_VMemory = new VMemViewStream();
                fontChanged += FontChanged;
                SizeChanged += DisassemblyView_SizeChanged;
                VMemoryOffset = Machine.Current.DeviceCPU.State.PC;
            }
        }

        void DisassemblyView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ComputerLineCount();
        }

        void FontChanged(object sender, EventArgs e)
        {
            if (Object.ReferenceEquals(sender, this))
            {
                ComputerLineCount();
            }
        }

        private void ComputerLineCount()
        {
            m_LineCount = (Int32)(ActualHeight / FontSize);
        }

        public void RefreshDisasm()
        {
            ReadDiasm();
        }

        private void ReadDiasm()
        {
            if (m_VMemory == null)
                return;

            m_VMemory.Position = Machine.Current.DeviceCPU.State.PC;
            MipsInstruction[] instructions = new MipsInstruction[m_LineCount];
            var beStream = new Int32SwapStream(m_VMemory);
            BinaryReader reader = new BinaryReader(beStream);

            for (Int32 i = 0; i < m_LineCount; i++)
            {
                instructions[i] = new MipsInstruction((UInt64)m_VMemory.Position, reader.ReadUInt32());
            }
            DisasmLines.Clear();


            foreach (var inst in instructions)
            {
                var line = new DiassemblyViewLine();
                line.LoadedInstruction = inst;

                DisasmLines.Add(line);
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
