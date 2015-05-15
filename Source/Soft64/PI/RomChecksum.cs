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
using System.Security.Cryptography;

namespace Soft64.PI
{
    public sealed class RomChecksum : HashAlgorithm
    {
        public const Int32 InputSize = 0x101000;

        private byte[] m_Hashcode = new byte[8];
        private CICKeyType m_CicType;

        public RomChecksum(CICKeyType type)
        {
            m_CicType = type;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            /* The source buffer must be big endian byte order, otherwise this fails */
            BinaryReader arrayReader = new BinaryReader(new MemoryStream(array));
            BinaryWriter hashWriter = new BinaryWriter(new MemoryStream(m_Hashcode));

            if (cbSize < InputSize)
                throw new ArgumentOutOfRangeException("cbSize must be 0x10100 or bigger");

            uint seed;

            switch (m_CicType)
            {
                case CICKeyType.CIC_X101:
                case CICKeyType.CIC_X102: seed = 0xF8CA4DDC; break;
                case CICKeyType.CIC_X103: seed = 0xA3886759; break;
                case CICKeyType.CIC_X105: seed = 0xDF26F436; break;
                case CICKeyType.CIC_X106: seed = 0x1FEA617A; break;
                default: return;
            }

            // Do the CIC-NUS checksum algorithm (found in n64sums.c)
            // Based on uCON64's N64 checksum algorithm by Andreas Stebbins
            // Copyright (0) 2005 Parasyte

            uint t1, t2, t3, t4, t5, t6;
            t1 = t2 = t3 = t4 = t5 = t6 = seed;
            uint read = 0;
            uint r = 0;

            arrayReader.BaseStream.Position = 0x1000;
            for (int i = 0; i < 0x100000; i += 4)
            {
                read = arrayReader.ReadUInt32();

                if ((t6 + read) < t6) t4++;
                t6 += read;
                t3 ^= read;
                r = RotateLeft(read, (int)(read & 0x1F));
                t5 += r;
                if (t2 > read) t2 ^= r;
                else t2 ^= t6 ^ read;

                if (m_CicType == CICKeyType.CIC_X105)
                {
                    long oldPos = arrayReader.BaseStream.Position;
                    arrayReader.BaseStream.Position = 0x40 + 0x0710 + (i & 0xFF);
                    t1 += arrayReader.ReadUInt32() ^ read;
                    arrayReader.BaseStream.Position = oldPos;
                }
                else
                {
                    t1 += t5 ^ read;
                }
            }

            if (m_CicType == CICKeyType.CIC_X103)
            {
                hashWriter.Write((UInt32)((t6 ^ t4) + t3));
                hashWriter.Write((UInt32)((t5 ^ t2) + t1));
            }
            else if (m_CicType == CICKeyType.CIC_X106)
            {
                hashWriter.Write((UInt32)((t6 * t4) + t3));
                hashWriter.Write((UInt32)((t5 * t2) + t1));
            }
            else
            {
                hashWriter.Write((UInt32)(t6 ^ t4 ^ t3));
                hashWriter.Write((UInt32)(t5 ^ t2 ^ t1));
            }
        }

        protected override byte[] HashFinal()
        {
            return m_Hashcode;
        }

        public override int HashSize
        {
            get
            {
                return m_Hashcode.Length;
            }
        }

        private uint RotateLeft(uint value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }

        public Int32 CRC1
        {
            get { return (m_Hashcode[3] | (m_Hashcode[2] << 8) | (m_Hashcode[1] << 16) | (m_Hashcode[0] << 24)); }
        }

        public Int32 CRC2
        {
            get { return (m_Hashcode[7] | (m_Hashcode[6] << 8) | (m_Hashcode[5] << 16) | (m_Hashcode[4] << 24)); }
        }

        public override void Initialize()
        {
        }
    }
}