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

namespace Soft64.PI
{
    public sealed class PiBusSpeedConfig
    {
        private Int32 m_Latency;
        private Int32 m_PulseWidth;
        private Int32 m_PageSize;
        private Int32 m_ReleaseTime;

        public PiBusSpeedConfig(Int32 latency, Int32 pulseWidth, Int32 pageSize, Int32 releaseTime)
        {
            m_Latency = latency;
            m_PulseWidth = pulseWidth;
            m_PageSize = pageSize;
            m_ReleaseTime = releaseTime;
        }

        public Int32 DeviceLatency { get { return m_Latency; } }

        public Int32 PulseWidth { get { return m_PulseWidth; } }

        public Int32 PageSize { get { return m_PageSize; } }

        public Int32 ReleaseTime { get { return m_ReleaseTime; } }

        public Int64 Config
        {
            get
            {
                return (m_PageSize << 24) | (m_PulseWidth << 16) | (m_ReleaseTime << 8) | m_Latency;
            }
        }
    }
}