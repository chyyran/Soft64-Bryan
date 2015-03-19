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
using NLog;

namespace Soft64.DeviceMemory
{
    public sealed class CartridgeChangedEventArgs : EventArgs
    {
        private Cartridge m_NewCartridge;

        public CartridgeChangedEventArgs(Cartridge newCartridge)
        {
            m_NewCartridge = newCartridge;
        }

        public Cartridge NewCartridge
        {
            get { return m_NewCartridge; }
        }
    }

    public sealed class ParallelInterface : Stream
    {
        private Cartridge m_CurrentCartridge;
        private DiskDrive m_CurrentDiskDrive;
        private PIStream m_PIDataStream;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<CartridgeChangedEventArgs> CartridgeChanged;

        public Boolean SkipRealDMA { get; set; }

        public ParallelInterface()
        {
            m_PIDataStream = new PIStream();
        }

        public void MountCartridge(Cartridge cartridge)
        {
            if (cartridge == null)
                throw new ArgumentNullException("cartridge");

            if (m_CurrentCartridge != null)
                ReleaseCartridge();

            m_CurrentCartridge = cartridge;
            m_CurrentCartridge.Open();
            m_PIDataStream.MountCartridge(cartridge);

            OnCartridgeChanged(cartridge);

            logger.Trace("A catridge has been inserted into the slot");
            logger.Trace("Cartridge Rom Header ==========================");
            logger.Trace("Name:         " + m_CurrentCartridge.RomImage.Name);
            logger.Trace("Serial:       " + m_CurrentCartridge.RomImage.Serial.ToString());
            logger.Trace("Computed CIC: " + m_CurrentCartridge.RomImage.BootRomInformation.CIC.GetGoodName());
            logger.Trace("Linked CIC:   " + ((m_CurrentCartridge.LockoutKey != null) ? m_CurrentCartridge.LockoutKey.ToString() : "None"));
            logger.Trace("CRC1:         " + m_CurrentCartridge.RomImage.CRC1.ToString("X8"));
            logger.Trace("CRC2:         " + m_CurrentCartridge.RomImage.CRC2.ToString("X8"));
            logger.Trace("Region:       " + m_CurrentCartridge.RomImage.Region.ToString());
            logger.Trace("VI Rate:      " + m_CurrentCartridge.RomImage.GetVIRate().ToString());
        }

        public void ReleaseCartridge()
        {
            if (m_CurrentCartridge != null)
            {
                m_PIDataStream.UnmountCartridge();
                m_CurrentCartridge = null;
                OnCartridgeChanged(null);
            }
        }

        public void MountDiskDrive(DiskDrive drive)
        {
            logger.Warn("This method is more of a stub than something functional");

            m_CurrentDiskDrive = drive;
            m_PIDataStream.MountDiskDrive(drive);
            logger.Debug("A disk drive has been inserted into the slot");
        }

        public void ReleaseDiskDrive()
        {
            logger.Warn("This method is more of a stub than something functional");
            logger.Debug("Releasing current disk drive");

            m_CurrentDiskDrive = null;
        }

        public Cartridge InsertedCartridge
        {
            get { return m_CurrentCartridge; }
        }

        public DiskDrive InsertedDiskDrive
        {
            get { return m_CurrentDiskDrive; }
        }

        private void OnCartridgeChanged(Cartridge cartridge)
        {
            var e = CartridgeChanged;

            if (e != null)
            {
                e(this, new CartridgeChangedEventArgs(cartridge));
            }
        }

        public override bool CanWrite
        {
            get { return m_PIDataStream.CanWrite; }
        }

        public override long Length
        {
            get { return m_PIDataStream.Length; }
        }

        public override long Position
        {
            get
            {
                return m_PIDataStream.Position;
            }
            set
            {
                m_PIDataStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_PIDataStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_PIDataStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            m_PIDataStream.Flush();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return "Parellel Interface";
        }
    }
}