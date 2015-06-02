using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Machine.Current.DeviceRCP.ExecuteN64MemoryOpSafe((s) =>
            {
                s.Flush();
            });
        }

        public override long Length
        {
            get
            {
                Int64 length = 0;

                Machine.Current.DeviceRCP.ExecuteN64MemoryOpSafe((s) =>
                {
                    length = s.Length;
                });

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

            Machine.Current.DeviceRCP.ExecuteN64MemoryOpSafe((s) =>
            {
                s.Position = m_Position;
                read = s.Read(buffer, offset, count);
            });

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Int64 pos = -1;

            Machine.Current.DeviceRCP.ExecuteN64MemoryOpSafe((s) =>
            {
                pos = s.Seek(offset, origin);
            });

            return pos;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Machine.Current.DeviceRCP.ExecuteN64MemoryOpSafe((s) =>
            {
                s.Position = m_Position;
                s.Write(buffer, offset, count);
            });
        }
    }
}
