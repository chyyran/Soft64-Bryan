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
using System.Collections.Generic;
using Soft64.PI;

namespace Soft64
{
    public sealed class CartridgeInfo : IEnumerable<String>
    {
        public CartridgeInfo(Cartridge cartridge)
        {
            BusConfig = cartridge.RomImage.BusConfig;
            EntryPoint = cartridge.RomImage.EntryPoint;
            Clockrate = cartridge.RomImage.Clockrate;
            HashCRC1 = cartridge.RomImage.CRC1;
            HashCRC2 = cartridge.RomImage.CRC2;
            Name = cartridge.RomImage.Name;
            RegionCode = cartridge.RomImage.Region;
            Release = cartridge.RomImage.Release;
            Serial = cartridge.RomImage.Serial;
        }

        public PiBusSpeedConfig BusConfig { get; private set; }

        public String Name { get; private set; }

        public Int32 Clockrate { get; private set; }

        public Int64 EntryPoint { get; private set; }

        public Int32 Release { get; private set; }

        public GameSerial Serial { get; private set; }

        public Int32 HashCRC1 { get; private set; }

        public Int32 HashCRC2 { get; private set; }

        public RegionType RegionCode { get; private set; }

        public IEnumerator<string> GetEnumerator()
        {
            List<String> props = new List<String>();

            props.AddRange(
                new[] {
                    "PI Config: 0x" + BusConfig.Config.ToString("X8"),
                    "ROM Name: " + Name,
                    "Clock Rate: " + new TimeSpan(Clockrate).TotalSeconds.ToString(),
                    "Boot Offset: 0x" + EntryPoint.ToString("X8"),
                    "Release Offset: 0x" + Release.ToString("X8"),
                    "ROM Serial: " + Serial.ToString(),
                    "Checksum 1: 0x" + HashCRC1.ToString("X8"),
                    "Checksum 2: 0x" + HashCRC2.ToString("X8"),
                    "TV Standard: " + RegionCode.ToString()
                });

            return props.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}