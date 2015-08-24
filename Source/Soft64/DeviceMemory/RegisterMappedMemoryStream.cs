using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.DeviceMemory
{
    public abstract class RegisterMappedMemoryStream : Stream
    {
        private Int32 m_MemorySize;
        private Byte[] m_Buffer;
        private Int64 m_Position;
        

        protected RegisterMappedMemoryStream(Int32 memorySize)
        {
            m_MemorySize = memorySize;
            m_Buffer = new Byte[memorySize];
        }

        protected UInt32 ReadUInt32(Int32 offset)
        {
            unsafe
            {
                fixed (byte * ptrBuffer = m_Buffer)
                {
                    return *(uint*)(byte *)(ptrBuffer + offset);
                }
            }
        }

        protected void WriteUInt32(Int32 offset, UInt32 value)
        {
            unsafe
            {
                fixed (byte * ptrBuffer = m_Buffer)
                {
                    *((uint *)(byte *)(ptrBuffer + offset)) = value;
                }
            }
        }

        protected Byte ReadByte(Int32 offset)
        {
            return m_Buffer[offset];
        }

        protected void WriteByte(Int32 offset, Byte value)
        {
            m_Buffer[offset] = value;
        }

        protected UInt16 ReadUInt16(Int32 offset)
        {
            unsafe
            {
                fixed (byte* ptrBuffer = m_Buffer)
                {
                    return *(ushort*)(byte *)(ptrBuffer + offset);
                }
            }
        }

        protected void WriteUInt16(Int32 offset, UInt16 value)
        {
            unsafe
            {
                fixed (byte* ptrBuffer = m_Buffer)
                {
                    *((ushort*)(byte *)(ptrBuffer + offset)) = value;
                }
            }
        }

        protected Int32 ReadInt32(Int32 offset)
        {
            return (Int32)ReadUInt32(offset);
        }

        protected void WriteInt32(Int32 offset, Int32 value)
        {
            WriteUInt32(offset, (UInt32)value);
        }

        protected Int16 ReadInt16(Int32 offset)
        {
            return (Int16)ReadUInt16(offset);
        }

        protected void WriteInt16(Int32 offset, Int16 value)
        {
            WriteUInt16(offset, (UInt16)value);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return m_MemorySize; }
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
            for (Int32 i = 0; i < count; i++)
            {
                buffer[offset + i] = m_Buffer[(Int32)Position++];
            }

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (Int32 i = 0; i < count; i++)
            {
                m_Buffer[(Int32)Position++] = buffer[offset + i];
            }
        }
    }
}
