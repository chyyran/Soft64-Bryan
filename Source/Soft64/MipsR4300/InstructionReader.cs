using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64.IO;
using Soft64.MipsR4300.Debugging;

namespace Soft64.MipsR4300
{
    public enum MemoryAccessMode
    {
        /// <summary>
        /// Read only physical memory
        /// </summary>
        Physical,

        /// <summary>
        /// Read using with full blown MMU support
        /// </summary>
        Virtual,

        /// <summary>
        /// Used for debugging purposes
        /// </summary>
        DebugVirtual,

        /// <summary>
        /// Read virtual memory without cache support
        /// </summary>
        VirtualWithoutCache
    }

    public class InstructionReader : IDisposable
    {
        private Stream m_Source;
        private Boolean m_ModeInvalid;
        private BinaryReader m_BinReader;
        private Int64 m_Position;
        private Boolean m_Disposed;
        private Int32 m_ReadHi, m_ReadLo;

        public InstructionReader(MemoryAccessMode accessMode)
        {
            switch (accessMode)
            {
                case MemoryAccessMode.Physical:
                    m_Source = Machine.Current.N64Memory; break;

                case MemoryAccessMode.DebugVirtual:
                    m_Source = new VMemViewStream(); break;

                case MemoryAccessMode.Virtual:
                    m_Source = Machine.Current.DeviceCPU.VirtualMemoryStream; break;

                default: m_ModeInvalid = true; break;
            }

            if (m_Source != null)
                m_BinReader = new BinaryReader(new Int32SwapStream(m_Source));
        }

        public Int64 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public MipsInstruction ReadInstruction()
        {
            if (m_ModeInvalid)
                throw new InvalidOperationException("Cannot read instruction with unsupported memory mode");

            if (m_Disposed)
                throw new ObjectDisposedException(this.ToString());

            m_Source.Position = Position;
            UInt32 read = m_BinReader.ReadUInt32();

            m_ReadHi = (Int32)(read & 0xFF00) >> 16;
            m_ReadLo = (Int32)(read & 0x00FF);

            MipsInstruction inst = new MipsInstruction(Position, read);
            Position += 4;
            return inst;
        }

        public Int32 ReadHi { get { return m_ReadHi; } }

        public Int32 ReadLo { get { return m_ReadLo; } }

        private void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    m_BinReader.Dispose();
                    m_Source = null;
                }

                m_Disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
