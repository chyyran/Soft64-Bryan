using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public unsafe sealed class MipsDataManipulator : IDisposable
    {
        private Stream m_SourceStream;
        private Byte[] m_Buffer;
        private UInt16* m_Ptr16;
        private UInt32* m_Ptr32;
        private UInt64* m_Ptr64;
        private GCHandle m_PinHandle;
        private Boolean m_Disposed;

        public MipsDataManipulator(Stream sourceStream)
        {
            m_SourceStream = sourceStream;
            m_Buffer = new Byte[8];

            /* Pin the buffer in memory */
            m_PinHandle = GCHandle.Alloc(m_Buffer, GCHandleType.Pinned);
            
            /* Now get pointers of the pinned object */
            IntPtr pointer = m_PinHandle.AddrOfPinnedObject();

            unsafe
            {
                m_Ptr16 = (UInt16*)pointer;
                m_Ptr32 = (UInt32*)pointer;
                m_Ptr64 = (UInt64*)pointer;
            }
            
        }

        public UInt64 LoadDoublewordUnsigned(Int64 address)
        {
            m_SourceStream.Position = address;
            m_SourceStream.Read(m_Buffer, 0, 8);

            unsafe
            {
                return *m_Ptr64;
            }
        }

        public void Store(Int64 address, UInt64 value)
        {
            unsafe
            {
                *m_Ptr64 = value;
            }

            m_SourceStream.Position = address;
            m_SourceStream.Write(m_Buffer, 0, 8);
        }

        public UInt32 LoadWordUnsigned(Int64 address)
        {
            m_SourceStream.Position = address;
            m_SourceStream.Read(m_Buffer, 0, 4);

            unsafe
            {
                return *m_Ptr32;
            }
        }

        public void Store(Int64 address, UInt32 value)
        {
            unsafe
            {
                *m_Ptr32 = value;
            }

            m_SourceStream.Position = address;
            m_SourceStream.Write(m_Buffer, 0, 4);
        }

        public UInt16 LoadHalfwordUnsigned(Int64 address)
        {
            m_SourceStream.Position = address;
            m_SourceStream.Read(m_Buffer, 0, 2);

            unsafe
            {
                return *m_Ptr16;
            }
        }

        public void Store(Int64 address, UInt16 value)
        {
            unsafe
            {
                *m_Ptr16 = value;
            }

            m_SourceStream.Position = address;
            m_SourceStream.Write(m_Buffer, 0, 2);
        }

        public Int64 LoadDoublewordSigned(Int64 address)
        {
            return (Int64)LoadDoublewordUnsigned(address);
        }

        public void Store(Int64 address, Int64 value)
        {
            Store(address, (UInt64)value);
        }

        public Int32 LoadWordSigned(Int64 address)
        {
            return (Int32)LoadWordUnsigned(address);
        }

        public void Store(Int64 address, Int32 value)
        {
            Store(address, (UInt32)value);
        }

        public Int16 LoadHalfwordSigned(Int64 address)
        {
            return (Int16)LoadHalfwordUnsigned(address);
        }

        public void Store(Int64 address, Int16 value)
        {
            Store(address, (UInt16)value);
        }

        public Byte LoadByteUnsigned(Int64 address)
        {
            m_SourceStream.Position = address;
            return (Byte)m_SourceStream.ReadByte();
        }

        public void Store(Int64 address, Byte value)
        {
            m_SourceStream.Position = address;
            m_SourceStream.WriteByte(value);
        }

        public SByte LoadByteSigned(Int64 address)
        {
            return (SByte)LoadByteUnsigned(address);
        }

        public void Store(Int64 address, SByte value)
        {
            Store(address, (Byte)value);
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    
                }

                m_PinHandle.Free();

                m_Disposed = true;
            }
        }

        #endregion
    }
}
