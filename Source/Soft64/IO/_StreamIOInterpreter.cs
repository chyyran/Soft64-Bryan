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
using System.Linq;

namespace Soft64.IO
{
    internal static class _StreamIOInterpreter
    {
        public static void RunOperation(_IOTransfer _IOTransfer, _StreamQuery streamQuery, Boolean isWriteMode, _StreamOp operation)
        {
            Int32 index = 0;
            var list = streamQuery.StreamQuery;
            Int32 count = list.Count();
            Int64 address = streamQuery.RequestedAddress;

            foreach (var entry in list)
            {
                /* Skip entries that do not allow write operations */
                if (isWriteMode && !entry.Value.CanWrite)
                    continue;

                Boolean inZone = CheckInReadZone(streamQuery.RequestedAddress, _IOTransfer.Count, entry.Key, entry.Value);
                _IOTransfer request = _IOTransfer;

                if (index == 0)
                {
                    /* On first stream, determine whether if it is partially or completely inside the reading zone */
                    if (inZone)
                    {
                        /* Copy the whole stream into the buffer */
                        entry.Value.Position = 0;
                        request = new _IOTransfer(_IOTransfer.Buffer, _IOTransfer.Offset, (Int32)entry.Value.Length);
                    }
                    else
                    {
                        /* Only partially copy the steam into the buffer */
                        entry.Value.Position = ConvertToOffsetInStream(address, entry.Key);
                        request = new _IOTransfer(_IOTransfer.Buffer, _IOTransfer.Offset, _IOTransfer.Count);
                    }
                }
                else if (index == count - 1)
                {
                    /* If we are on the last stream, determine whether its inside partially or completely */
                    if (inZone)
                    {
                        /* Copy the whole stream into the buffer */
                        entry.Value.Position = 0;
                        request = new _IOTransfer(_IOTransfer.Buffer, ConvertToOffsetInBuffer(address, entry.Key) + _IOTransfer.Offset, (Int32)entry.Value.Length);
                    }
                    else
                    {
                        /* Partially copy the steam into the buffer */
                        entry.Value.Position = 0;
                        Int32 bufferOffset = ConvertToOffsetInBuffer(streamQuery.RequestedAddress, entry.Key) + _IOTransfer.Offset;
                        request = new _IOTransfer(_IOTransfer.Buffer, bufferOffset, _IOTransfer.Count - bufferOffset);
                    }
                }
                else
                {
                    /* At this point, the stream is completely in the reading zone, so alwasy do a full read */
                    entry.Value.Position = 0;
                    request = new _IOTransfer(_IOTransfer.Buffer, ConvertToOffsetInBuffer(address, entry.Key) + _IOTransfer.Offset, (Int32)entry.Value.Length);
                }

                operation(request, entry.Value);
                index++;
            }
        }

        public static Boolean CheckInReadZone(Int64 requestedAddress, Int32 count, Int64 streamBaseOffset, Stream stream)
        {
            var streamEnd = streamBaseOffset + stream.Length;

            if (streamBaseOffset >= requestedAddress && streamEnd < (requestedAddress + count))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Int64 ConvertToOffsetInStream(Int64 address, Int64 streamMemoryAddress)
        {
            return address - streamMemoryAddress;
        }

        public static Int32 ConvertToOffsetInBuffer(Int64 memoryPosition, Int64 streamMemoryPosition)
        {
            return (Int32)(streamMemoryPosition - memoryPosition);
        }
    }
}