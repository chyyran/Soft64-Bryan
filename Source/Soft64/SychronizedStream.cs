using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64
{
    internal sealed class SychronizedStream : Stream
    {
        private Stream m_ThreadUnsafeStream;
        private Object m_StreamLock;

        [ThreadStatic]
        private static Int64 s_Position; /* Each thread will have their own position variable */

        public SychronizedStream(Stream sourceStream)
        {
            m_ThreadUnsafeStream = sourceStream;
            m_StreamLock = new Object();
        }

        public override bool CanRead
        {
            get { return m_ThreadUnsafeStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_ThreadUnsafeStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_ThreadUnsafeStream.CanWrite; }
        }

        public override void Flush()
        {
            lock (m_StreamLock)
            {
                m_ThreadUnsafeStream.Flush();
            }
        }

        public override long Length
        {
            get { return m_ThreadUnsafeStream.Length; }
        }

        public override long Position
        {
            get
            {
                return s_Position;
            }
            set
            {
                s_Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (m_StreamLock)
            {
                m_ThreadUnsafeStream.Position = s_Position;
                return m_ThreadUnsafeStream.Read(buffer, offset, count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_ThreadUnsafeStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            lock (m_StreamLock)
            {
                m_ThreadUnsafeStream.SetLength(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (m_StreamLock)
            {
                m_ThreadUnsafeStream.Position = s_Position;
                m_ThreadUnsafeStream.Write(buffer, offset, count);
            }
        }
    }
}
