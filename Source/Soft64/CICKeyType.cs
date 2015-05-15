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

namespace Soft64
{
    public enum CICKeyType
    {
        Unknown,
        CIC_X101,
        CIC_X102,
        CIC_X103,
        CIC_X105,
        CIC_X106,
        CIC_HLE
    }

    public static class CICKeyTypeExtensions
    {
        public static String GetGoodName(this CICKeyType type)
        {
            switch (type)
            {
                case CICKeyType.CIC_HLE: return "HLE CIC";
                case CICKeyType.CIC_X101: return "CIC-X101";
                case CICKeyType.CIC_X102: return "CIC-X102";
                case CICKeyType.CIC_X103: return "CIC-X103";
                case CICKeyType.CIC_X105: return "CIC-X105";
                case CICKeyType.CIC_X106: return "CIC-X106";
                default: return type.ToString();
            }
        }

        public static Int32 Seed(this CICKeyType cic)
        {
            switch (cic)
            {
                default: return 0;
                case CICKeyType.CIC_X101:
                case CICKeyType.CIC_X102: return 0x3F;
                case CICKeyType.CIC_X103: return 0x78;
                case CICKeyType.CIC_X105: return 0x91;
                case CICKeyType.CIC_X106: return 0x85;
            }
        }
    }
}