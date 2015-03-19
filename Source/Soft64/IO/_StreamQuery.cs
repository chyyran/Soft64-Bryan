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

namespace Soft64.IO
{
    public sealed class _StreamQuery : IEnumerable<KeyValuePair<Int64, Stream>>
    {
        private Int64 m_RequestedAddress;
        private IEnumerable<KeyValuePair<Int64, Stream>> m_Query;

        public _StreamQuery(Int64 requestedAddress, IEnumerable<KeyValuePair<Int64, Stream>> streamQuery)
        {
            m_RequestedAddress = requestedAddress;
            m_Query = streamQuery;
        }

        #region IEnumerable<KeyValuePair<long,Stream>> Members

        public IEnumerator<KeyValuePair<long, Stream>> GetEnumerator()
        {
            return m_Query.GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<long,Stream>> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_Query.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IStreamQuery Members

        public Int64 RequestedAddress
        {
            get { return m_RequestedAddress; }
        }

        public IEnumerable<KeyValuePair<long, Stream>> StreamQuery
        {
            get
            {
                foreach (var entry in m_Query)
                {
                    yield return new KeyValuePair<Int64, Stream>(entry.Key, entry.Value as Stream);
                }
            }
        }

        #endregion IStreamQuery Members
    }
}