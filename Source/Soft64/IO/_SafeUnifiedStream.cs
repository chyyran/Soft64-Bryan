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

using System.IO;
using System.Runtime.CompilerServices;

namespace Soft64.IO
{
    internal sealed class _SafeUnifiedStream : Stream
    {
        private UnifiedStream m_UnifiedStream;

        public _SafeUnifiedStream(UnifiedStream stream)
        {
            m_UnifiedStream = stream;
        }

        public override bool CanRead
        {
            get { return m_UnifiedStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_UnifiedStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_UnifiedStream.CanSeek; }
        }

        public override void Flush()
        {
            m_UnifiedStream.Flush();
        }

        public override long Length
        {
            get { return m_UnifiedStream.Length; }
        }

        public override long Position
        {
            get
            {
                return m_UnifiedStream.Position;
            }
            set
            {
                m_UnifiedStream.Position = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
        {
            m_UnifiedStream.UseCompiler = false;
            var read = m_UnifiedStream.Read(buffer, offset, count);
            m_UnifiedStream.UseCompiler = true;
            return read;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_UnifiedStream.Seek(offset, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetLength(long value)
        {
            m_UnifiedStream.SetLength(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
        {
            m_UnifiedStream.UseCompiler = false;
            m_UnifiedStream.Write(buffer, offset, count);
            m_UnifiedStream.UseCompiler = true;
        }
    }
}