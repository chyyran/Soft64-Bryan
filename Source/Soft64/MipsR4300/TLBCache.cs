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
using System.Linq;

/* TODO: Ditch the entyr info class and use raw TLB entries ? */
/* TODO: Improve the event so its clear it only fires when the CPU calls TLB write instructions */

namespace Soft64.MipsR4300
{
    public class TLBCache : IEnumerable<TLBEntryInfo>, IList<TLBEntry>
    {
        private CP0Registers m_Cp0Regs;
        private ExecutionState m_MipsState;
        private TLBEntry[] m_Entries;
        private Dictionary<VirtualPageNumber2, TLBEntry> m_VPN2Dictionary = new Dictionary<VirtualPageNumber2, TLBEntry>();
        private Dictionary<VirtualPageNumber2, TLBEntry> m_GlobalVPN2Dictionary = new Dictionary<VirtualPageNumber2, TLBEntry>();
        private Boolean m_DisableTLB = false; // Used to simulate a real TLB shutdown
        private Boolean m_DuplicateEntryError = false;

        /* Debug Events */

        public event EventHandler<TLBCacheChangeEventArgs> CacheChanged;

        public TLBCache(ExecutionState state)
        {
            m_MipsState = state;
            m_Cp0Regs = state.CP0Regs;
            m_Entries = new TLBEntry[48];
        }

        public virtual void Initialize()
        {
            m_Cp0Regs.BadVAddr = 0xFFFFFFFF;
            m_Cp0Regs.Random = 31;
            ClearEntries();
            m_VPN2Dictionary.Clear();
            m_GlobalVPN2Dictionary.Clear();
        }

        /// <summary>
        /// If successful, the probe returns the entry index matching the VPN2 / ASID values in EntryHi.
        /// </summary>
        /// <remarks>
        /// Ref: MIPS R4300I Manual Page 637
        /// Ref: MIPS R4300I Manual Page 115
        /// </remarks>
        public virtual void Probe()
        {
            if (m_DisableTLB)
                return;

            UpdateRandom();

            VirtualPageNumber2 vpn2 = new VirtualPageNumber2(m_Cp0Regs.EntryHi);

            if (ContainsEntry(vpn2))
            {
                /* Get the entry index */
                m_Cp0Regs.Index = GetIndex(vpn2);
            }
            else
            {
                /* Set P bit to 1 indicating the probe has failed */
                m_Cp0Regs.Index = 0x80000000;
            }
        }

        /// <summary>
        /// Writes an entry to the TLB.
        /// Registers used: EntryHi, PageMask, EntryLo0, EntryLo1, Index, Random, Wired, BadVAddr
        /// </summary>
        public virtual void Write()
        {
            if (m_DisableTLB) return;

            if (!CheckSafeEntry((Byte)m_Cp0Regs.Index))
            {
                UpdateRandom();
                AddEntry((Int32)m_Cp0Regs.Index, CreateEntryFromRegs());
            }
            else
            {
                throw new TLBException(TLBExceptionType.Mod);
            }

            OnCacheChanged((Int32)m_Cp0Regs.Index);
        }

        /// <summary>
        /// Writes a TLB entry at a random location.
        /// </summary>
        public virtual void WriteRandom()
        {
            if (m_DisableTLB) return;
            UpdateRandom();
            AddEntry((Int32)m_Cp0Regs.Random, CreateEntryFromRegs());
            OnCacheChanged((Int32)m_Cp0Regs.Index);
        }

        public virtual void Read()
        {
            if (m_DisableTLB) return;

            UpdateRandom();

            TLBEntry entry = GetEntry((Int32)m_Cp0Regs.Index);

            if (entry != null)
            {
                m_Cp0Regs.PageMask = entry.PageMask;
                m_Cp0Regs.EntryHi = entry.VPN2.EntryHi;
                m_Cp0Regs.EntryLo0 = entry.PfnOdd;
                m_Cp0Regs.EntryLo1 = entry.PfnEven;
            }
        }

        public void FlushAll()
        {
            if (m_DisableTLB) return;

            Initialize();
        }

        public void RemoveEntry(int index)
        {
            m_VPN2Dictionary.Remove(m_Entries[index].VPN2);
            m_Entries[index] = null;
            OnCacheChanged(index);
        }

        public IEnumerator<TLBEntryInfo> GetEnumerator()
        {
            for (Int32 i = 0; i < 48; i++)
            {
                if (GetEntry(i) != null)
                {
                    yield return new TLBEntryInfo(i, GetEntry(i));
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public long TranslateVirtualAddress(long virtualAddress)
        {
            VirtualPageNumber2 vpn2 = new VirtualPageNumber2(m_Cp0Regs.EntryHi);
            PageSize pageSize = new PageSize(m_Cp0Regs.PageMask);
            TLBEntry entry = null;

            /* Note:
             * We are assuming the software set ASID and VPN in the EntryHi when comparing
             * the virtual address with the TLB cache.  Without a real test of this,
             * we don't know truly know if we rely on EntryHi every time there is a translation
             * to be performed.  I do not understand how the TLB is really even used on the N64
             * system, virtual memory is useless without having some kind of pagefile.
             */

            if (m_DuplicateEntryError)
            {
                // TODO: Set the TS bit in the status register
                m_DisableTLB = true;
                return -1;
            }

            /* Access the global entries first */
            if (!m_GlobalVPN2Dictionary.TryGetValue(vpn2, out entry))
            {
                /* If nothing is in global table, then access the entries grouped by an ASID */
                if (!m_VPN2Dictionary.TryGetValue(vpn2, out entry))
                    throw new TLBException(TLBExceptionType.Refill);
            }

            Boolean isOddPage = ((virtualAddress & pageSize.Size) > 0) ? false : true;
            PageFrameNumber pfn = isOddPage ? entry.PfnOdd : entry.PfnEven;

            if (pfn.IsValid)
                return pfn.MapPhysical(pageSize, (UInt64)virtualAddress);
            else
                throw new TLBException(TLBExceptionType.Refill);
        }

        public void AddEntry(int index, TLBEntry entry)
        {
            m_Entries[index] = entry;

            try
            {
                /* Is the entry global? */
                if (entry.IsGlobal)
                {
                    m_GlobalVPN2Dictionary.Add(entry.VPN2, entry);
                }
                else
                    m_VPN2Dictionary.Add(entry.VPN2, entry);
            }
            catch
            {
                m_DuplicateEntryError = true;
            }

            UpdateRandom();
            OnCacheChanged(index);
        }

        private void ClearEntries()
        {
            Array.Clear(m_Entries, 0, m_Entries.Length);
            m_VPN2Dictionary.Clear();
            m_GlobalVPN2Dictionary.Clear();
            OnCacheChanged(-1);
        }

        private void SetWired(Int32 wired)
        {
            if (m_DisableTLB) return;

            m_Cp0Regs.Wired = (UInt32)wired;
            m_Cp0Regs.Random = 46;
        }

        private bool ContainsEntry(VirtualPageNumber2 vpn2)
        {
            return m_VPN2Dictionary.ContainsKey(vpn2);
        }

        public byte GetIndex(VirtualPageNumber2 vpn2)
        {
            return (Byte)
                (from index in Enumerable.Range(0, m_Entries.Length)
                 let entry = m_Entries[index]
                 where entry != null && entry.Equals(vpn2)
                 select index).First();
        }

        private TLBEntry GetEntry(int index)
        {
            if (index < 0 || index >= m_Entries.Length)
                return null;

            return m_Entries[index];
        }

        private TLBEntry CreateEntryFromRegs()
        {
            TLBEntry entry = new TLBEntry(m_MipsState.Addressing64BitMode);
            entry.PageMask = m_Cp0Regs.PageMask;
            entry.VPN2 = new VirtualPageNumber2(m_Cp0Regs.EntryHi);
            entry.PfnOdd = m_Cp0Regs.EntryLo0;
            entry.PfnEven = m_Cp0Regs.EntryLo1;
            return entry;
        }

        private Boolean CheckSafeEntry(Byte index)
        {
            return index <= m_Cp0Regs.Wired;
        }

        private void UpdateRandom()
        {
            if (CheckSafeEntry((Byte)m_Cp0Regs.Random))
            {
                m_Cp0Regs.Random = m_Cp0Regs.Wired + 1;
            }
            else
            {
                m_Cp0Regs.Random++;

                if (m_Cp0Regs.Random >= 47)
                    m_Cp0Regs.Random = m_Cp0Regs.Wired + 1;
            }
        }

        public UInt64 PageMask
        {
            get { return m_Cp0Regs.PageMask; }
            set { m_Cp0Regs.PageMask = value; }
        }

        public UInt64 EntryHi
        {
            get { return m_Cp0Regs.EntryHi; }
            set { m_Cp0Regs.EntryHi = value; }
        }

        public UInt64 EntryLo0
        {
            get { return m_Cp0Regs.EntryLo0; }
            set { m_Cp0Regs.EntryLo0 = value; }
        }

        public UInt64 EntryLo1
        {
            get { return m_Cp0Regs.EntryLo1; }
            set { m_Cp0Regs.EntryLo1 = value; }
        }

        public UInt64 Index
        {
            get { return m_Cp0Regs.Index; }
            set { m_Cp0Regs.Index = value; }
        }

        public UInt64 Wired
        {
            get { return m_Cp0Regs.Wired; }
            set { SetWired((Int32)value); }
        }

        public UInt64 Random
        {
            get { return m_Cp0Regs.Random; }
        }

        public UInt64 BadVAddr
        {
            get { return m_Cp0Regs.BadVAddr; }
        }

        private void OnCacheChanged(Int32 index)
        {
            var e = CacheChanged;

            if (e != null)
                e(this, new TLBCacheChangeEventArgs(index));
        }

        #region IList<TLBEntry> Members

        public int IndexOf(TLBEntry item)
        {
            for (Int32 i = 0; i < m_Entries.Length; i++)
            {
                if (Object.Equals(item, m_Entries[i]))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, TLBEntry item)
        {
            m_Entries[index] = item;
        }

        public void RemoveAt(int index)
        {
            m_Entries[index] = null;
        }

        public TLBEntry this[int index]
        {
            get
            {
                return m_Entries[index];
            }
            set
            {
                m_Entries[index] = value;
            }
        }

        #endregion IList<TLBEntry> Members

        #region ICollection<TLBEntry> Members

        public void Add(TLBEntry item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TLBEntry item)
        {
            return m_Entries.Contains(item);
        }

        public void CopyTo(TLBEntry[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return m_Entries.Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TLBEntry item)
        {
            throw new NotSupportedException();
        }

        #endregion ICollection<TLBEntry> Members

        #region IEnumerable<TLBEntry> Members

        IEnumerator<TLBEntry> IEnumerable<TLBEntry>.GetEnumerator()
        {
            foreach (var entry in m_Entries)
                yield return entry;
        }

        #endregion IEnumerable<TLBEntry> Members
    }
}