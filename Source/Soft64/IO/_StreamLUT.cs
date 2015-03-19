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
using System.IO;
using System.Linq;

namespace Soft64.IO
{
    internal class _StreamLUT : IDictionary<Int64, Stream>
    {
        private SortedList<Int64, Stream> m_MemZones;

        public _StreamLUT()
        {
            m_MemZones = new SortedList<Int64, Stream>();
        }

        /* This queries and returns all the streams touching the reading zone */

        public IEnumerable<KeyValuePair<Int64, Stream>> QueryZoneTouches(_IORequest request)
        {
            var list = new List<KeyValuePair<Int64, Stream>>();
            var hit = false;

            foreach (var zone in m_MemZones)
            {
                var streamBegin = zone.Key;
                var streamEnd = streamBegin + zone.Value.Length;

                /* Check which streams are overlapped by the read block */
                if (hit && (streamBegin < request.ReadEnd))
                {
                    list.Add(new KeyValuePair<Int64, Stream>(zone.Key, zone.Value));
                }

                /* True if the stream contains the read offset, we hit the beginning of the region */
                if (request.Offset >= streamBegin && request.Offset < streamEnd)
                {
                    list.Add(new KeyValuePair<Int64, Stream>(zone.Key, zone.Value));
                    hit = true;
                }
            }

            return list;
        }

        #region IDictionary<long,Stream> Members

        public void Add(long key, Stream value)
        {
            if (!m_MemZones.ContainsKey(key))
                m_MemZones.Add(key, value);
            else
                throw new ArgumentException("Key already exists");
        }

        public bool ContainsKey(long key)
        {
            return m_MemZones.ContainsKey(key);
        }

        public ICollection<long> Keys
        {
            get { return m_MemZones.Keys; }
        }

        public bool Remove(long key)
        {
            return m_MemZones.Remove(key);
        }

        public bool TryGetValue(long key, out Stream value)
        {
            return m_MemZones.TryGetValue(key, out value);
        }

        public ICollection<Stream> Values
        {
            get { return m_MemZones.Values; }
        }

        public Stream this[long key]
        {
            get { return m_MemZones[key]; }
            set { m_MemZones[key] = value; }
        }

        #endregion IDictionary<long,Stream> Members

        #region ICollection<KeyValuePair<long,Stream>> Members

        public void Add(KeyValuePair<long, Stream> item)
        {
            m_MemZones.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_MemZones.Clear();
        }

        public bool Contains(KeyValuePair<long, Stream> item)
        {
            return m_MemZones.ContainsKey(item.Key) && m_MemZones.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<long, Stream>[] array, int arrayIndex)
        {
            /* TODO: This probably not a good way to do this...but idc */
            for (Int32 i = arrayIndex; i < array.Length; i++)
            {
                array[i] = new KeyValuePair<Int64, Stream>(Keys.ToArray()[i - arrayIndex], Values.ToArray()[i - arrayIndex]);
            }
        }

        public int Count
        {
            get { return m_MemZones.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<long, Stream> item)
        {
            /* TODO: I dont even bother with this now... */
            throw new NotImplementedException();
        }

        #endregion ICollection<KeyValuePair<long,Stream>> Members

        #region IEnumerable<KeyValuePair<long,Stream>> Members

        public IEnumerator<KeyValuePair<long, Stream>> GetEnumerator()
        {
            foreach (var pair in this)
            {
                yield return pair;
            }
        }

        #endregion IEnumerable<KeyValuePair<long,Stream>> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable Members
    }
}