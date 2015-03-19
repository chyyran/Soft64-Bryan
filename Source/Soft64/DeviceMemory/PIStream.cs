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
using Soft64.IO;

/* Memory layout notes for the parellel interface bus:
 * [0x00000000] Nothing?
 * [0x01000000] N64 Disk Drive (First 0x0100 bytes of the N64DD stream)
 * [0x02000000] N64 Disk Drive
 * [0x03000000] Flash Memory Status [Read-only] (Single 32-bit integer)
 * [0x03010000] Flash Memory Command [Write-only] (Single 32-bit integer)
 * [0x04000000] Nothing?
 * [0x0B000000] Rom Memory [Read-only]
 */

namespace Soft64.DeviceMemory
{
    /* For future: this class might be used to implement a cartridge domain system once we understand more about how it is used */

    internal sealed class PIStream : UnifiedStream
    {
        private const Int64 c_CartAddress = 0x0B000000;
        private Int64 m_CartSize;

        public void MountCartridge(Cartridge cartridge)
        {
            if (this.ContainsKey(c_CartAddress))
                throw new InvalidOperationException("There is a cartridge already mounted");

            m_CartSize = cartridge.PiCartridgeStream.Length;
            Add(c_CartAddress, cartridge.PiCartridgeStream);
        }

        public void UnmountCartridge()
        {
            if (this.ContainsKey(c_CartAddress))
                Remove(c_CartAddress);

            m_CartSize = 0;
        }

        public void MountDiskDrive(DiskDrive diskDrive)
        {
            throw new NotSupportedException();
        }

        public void UnmountDiskDrive()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                return c_CartAddress + m_CartSize;
            }
        }
    }
}