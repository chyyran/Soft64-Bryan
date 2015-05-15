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
using Soft64.PI;

namespace Soft64
{
    public sealed class VirtualCartridge : Cartridge
    {
        private Boolean m_FetchHeaderOnly;
        private Stream m_RomStream;
        private Stream m_FixedRomStream;

        public VirtualCartridge(Stream romImageStream)
            : this(romImageStream, false)
        {
        }

        public VirtualCartridge(Stream romImageStream, Boolean headerOnly)
        {
            m_RomStream = new _RomBlockStream(romImageStream);
            m_FetchHeaderOnly = headerOnly;
            IsReal = false;
        }

        public override void Open()
        {
            /* We create rom info from our buffer we copied from the file source */
            m_RomStream.Position = 0;
            RomImage = new CartRom(m_RomStream, m_FetchHeaderOnly, FormatVerifyFlag.Header);
            m_FixedRomStream = RomImage.RomStream;
            IsOpened = true;
        }

        public override void ColdReset()
        {
            PiCartridgeStream.Position = 0;
        }

        public override void NMIReset()
        {
            PiCartridgeStream.Position = 0;
        }

        public override Stream PiCartridgeStream
        {
            get { return m_FixedRomStream; }
        }

        public override string ToString()
        {
            return "Virtual Cartridge: " + this.RomImage.Name;
        }
    }
}