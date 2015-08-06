using System;
using System.IO;
using Soft64;

namespace Soft64Binding.WPF
{
    public sealed class N64StreamWrapper : Stream
    {
        private Int64 m_Position;

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            Machine.Current.N64MemorySafe.Flush();
        }

        public override long Length
        {
            get
            {
                Int64 length = 0;
                length = Machine.Current.N64MemorySafe.Length;
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Int32 read = -1;
            Machine.Current.N64MemorySafe.Position = m_Position;
            read = Machine.Current.N64MemorySafe.Read(buffer, offset, count);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Int64 pos = -1;
            pos = Machine.Current.N64MemorySafe.Seek(offset, origin);
            return pos;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Machine.Current.N64MemorySafe.Position = m_Position;
            Machine.Current.N64MemorySafe.Write(buffer, offset, count);
        }
    }
}