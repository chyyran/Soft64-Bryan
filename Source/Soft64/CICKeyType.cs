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
        CIC_NUS_6101,
        CIC_NUS_6102,
        CIC_NUS_6103,
        CIC_NUS_6105,
        CIC_NUS_6106,
        CIC_HLE
    }

    public static class CICKeyTypeExtensions
    {
        public static String GetGoodName(this CICKeyType type)
        {
            switch (type)
            {
                case CICKeyType.CIC_HLE: return "HLE CIC";
                case CICKeyType.CIC_NUS_6101: return "CIC-NUS-6101";
                case CICKeyType.CIC_NUS_6102: return "CIC-NUS-6102";
                case CICKeyType.CIC_NUS_6103: return "CIC-NUS-6103";
                case CICKeyType.CIC_NUS_6105: return "CIC-NUS-6105";
                case CICKeyType.CIC_NUS_6106: return "CIC-NUS-6106";
                default: return type.ToString();
            }
        }
    }
}