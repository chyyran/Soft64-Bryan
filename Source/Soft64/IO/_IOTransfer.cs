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

namespace Soft64.IO
{
    public struct _IOTransfer
    {
        private Byte[] m_Buffer;
        private Int32 m_BufferOffset;
        private Int32 m_Count;

        public _IOTransfer(Byte[] buffer, Int32 offset, Int32 count)
        {
            m_Buffer = buffer;
            m_BufferOffset = offset;
            m_Count = count;
        }

        public Byte[] Buffer
        {
            get { return m_Buffer; }
        }

        public Int32 Offset
        {
            get { return m_BufferOffset; }
        }

        public Int32 Count
        {
            get { return m_Count; }
        }
    }
}