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
using System.Linq;
using Soft64.Debugging;
using Soft64.IO;
using Soft64.MipsR4300.CP0;
using Soft64.MipsR4300.IO;

namespace Soft64.MipsR4300.Debugging
{
    public class VMemViewStream : UnifiedStream
    {
        private Object m_Lock = new object();

        public VMemViewStream()
        {
            UseCompiler = false;
            BuildMap();
            Machine.Current.DeviceCPU.Tlb.CacheChanged += TLB_CacheChanged;
        }

        private void BuildMap()
        {
            var pMap = new PhysicalMapStream();

            Add(0x80000000, new PhysicalMapStream());
            Add(0xA0000000, new PhysicalMapStream());

            /* Query all the valid entries in the TLB */
            var entries =
                    from entryInfo in Machine.Current.DeviceCPU.Tlb.AsQueryable<TLBEntryInfo>()
                    let tlbEntry = entryInfo.AssociatedEntry
                    select entryInfo;

            if (entries.Count() > 0)
            {
                foreach (var entryInfo in entries)
                {
                    if (entryInfo.AssociatedEntry.PfnOdd.IsValid)
                    {
                        Int64 addr = entryInfo.AssociatedEntry.PfnOdd.MapPhysical(entryInfo.AssociatedEntry.Size, 0);

                        Add(entryInfo.AssociatedEntry.MappedVirtualAddress,
                            new PMemMirrorStream(
                                addr,
                                entryInfo.AssociatedEntry.Size.Size));
                    }

                    if (entryInfo.AssociatedEntry.PfnEven.IsValid)
                    {
                        Int64 addr = entryInfo.AssociatedEntry.PfnEven.MapPhysical(entryInfo.AssociatedEntry.Size, 0);

                        Add(entryInfo.AssociatedEntry.MappedVirtualAddress,
                            new PMemMirrorStream(
                                addr,
                                entryInfo.AssociatedEntry.Size.Size));
                    }
                }
            }
        }

        private void TLB_CacheChanged(object sender, EventArgs e)
        {
            lock (m_Lock)
            {
                Clear();
                BuildMap();
            }
        }

        public override long Length
        {
            get { return 0xFFFFFFFFL + 1; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (m_Lock)
            {
                var c = base.Read(buffer, offset, count);
                return c;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (m_Lock)
            {
                base.Write(buffer, offset, count);
            }
        }

    }
}