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

namespace Soft64.PIF
{
    public sealed class PIFModule
    {
        private Stream m_PIFRam;
        private Stream m_PIFRom;

        /* TODO: Security Commands, Controller Commands, Flash Commands */

        public void Initialize()
        {
            m_PIFRam = new NamedMemoryStream("PIF Ram", 0x03F);
            m_PIFRom = new NamedMemoryStream("PIF Rom", 0x7BF);
        }

        public void Shutdown()
        {
            m_PIFRam.Dispose();
            m_PIFRam = null;

            m_PIFRom.Dispose();
            m_PIFRom = null;
        }

        public Stream RamStream
        {
            get { return m_PIFRam; }
        }

        public Stream RomStream
        {
            get { return m_PIFRom; }
        }

        internal class NamedMemoryStream : MemoryStream
        {
            private String m_Name;

            public NamedMemoryStream(String name, Int32 capacity)
                : base(capacity)
            {
                m_Name = name;
            }

            public override string ToString()
            {
                return m_Name;
            }
        }
    }
}