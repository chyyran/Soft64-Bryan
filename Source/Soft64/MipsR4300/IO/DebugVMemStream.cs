/*
Soft64 - C# N64 Emulator
Copyright (C) Soft64 Project @ Codeplex
Copyright (C) 2013 - 2014 Bryan Perris

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.IO;

namespace Soft64.MipsR4300.IO
{
    public sealed class DebugVMemStream : Stream
    {
        private VMemStream m_VmemStream;
        private Stream m_VMStream;

        public DebugVMemStream(VMemStream vmem)
        {
            m_VmemStream = vmem;
        }

        public override bool CanRead
        {
            get { return m_VmemStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_VmemStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_VmemStream.CanWrite; }
        }

        public override void Flush()
        {
            m_VmemStream.Flush();
        }

        public override long Length
        {
            get { return m_VmemStream.Length; }
        }

        public override long Position
        {
            get
            {
                return m_VmemStream.Position;
            }
            set
            {
                m_VmemStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            m_VmemStream.DebugMode = true;
            Int32 read = m_VmemStream.Read(buffer, offset, count);
            m_VmemStream.DebugMode = false;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_VmemStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_VmemStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_VmemStream.DebugMode = true;
            m_VmemStream.Write(buffer, offset, count);
            m_VmemStream.DebugMode = false;
        }
    }
}