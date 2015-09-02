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
    internal delegate void _StreamOp(_IOTransfer _IOTransfer, Stream stream);

    /// <summary>
    /// Provides a unified stream access to multiple mapped streams based on a static position key.
    /// </summary>
    public abstract class UnifiedStream : Stream, IDictionary<Int64, Stream>, IReadOnlyDictionary<Int64, String>
    {
        private Boolean m_Disposed;
        private Int64 m_Position;
        private Boolean m_UseCompiler = true;
        private _StreamLUT m_StreamLUT;
        private Object m_IOLock = new object();
        private _StreamIOCompiler m_OpCompiler;
        private _SafeUnifiedStream m_SafeStream;
        private const Int32 CACHE_MAX = 450;

        protected UnifiedStream()
        {
            m_StreamLUT = new _StreamLUT();
            m_OpCompiler = new _StreamIOCompiler();
            m_SafeStream = new _SafeUnifiedStream(this);
        }

        public Stream GetSafeStream()
        {
            return m_SafeStream;
        }

        #region Stream Implementation Memebers

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public abstract override long Length { get; }

        public override long Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, buffer.Length - offset);

            lock (m_IOLock)
            {
                var query = QueryStreams(Position, offset, count);
                var request = new _IOTransfer(buffer, offset, count);
                ExecuteMemoryOperation(request, query, true, (r, s) => StreamOp(r, s, StreamRead));
            }

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: return offset;
                case SeekOrigin.End: return Length - offset;
                case SeekOrigin.Current: return Position + offset;
                default: return -1;
            }
        }

        public sealed override void SetLength(long value)
        {
            throw new NotSupportedException("This class only supports fixed sized maps");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (m_IOLock)
            {
                var query = QueryStreams(Position, offset, count);
                var request = new _IOTransfer(buffer, offset, count);
                ExecuteMemoryOperation(request, query, true, (r, s) => StreamOp(r, s, StreamWrite));
            }
        }

        #endregion Stream Implementation Memebers

        #region Stream Utilites

        private void ExecuteMemoryOperation(_IOTransfer request, _StreamQuery query, Boolean writeMode, _StreamOp op)
        {
            if (!m_UseCompiler)
            {
                if (query.StreamQuery.Count() > 0)
                    _StreamIOInterpreter.RunOperation(request, query, writeMode, op);
            }
            else
            {
                if (m_OpCompiler.CacheCount > CACHE_MAX)
                {
                    m_OpCompiler.ClearCache();
                }

                m_OpCompiler.ExecuteOperation(request, query, writeMode, op);
            }

            /* Increment the position */
            m_Position += request.Count;
        }

        private _StreamQuery QueryStreams(Int64 position, Int32 bufferOffset, Int32 count)
        {
            if (!UseCompiler)
            {
                return new _StreamQuery(position, m_StreamLUT.QueryZoneTouches(new _IORequest(position, count)));
            }
            else
            {
                if (m_UseCompiler && m_OpCompiler.ContainsOperation(position, bufferOffset, count))
                    return new _StreamQuery(position, new KeyValuePair<Int64, Stream>[] { });
                else
                    return new _StreamQuery(position, m_StreamLUT.QueryZoneTouches(new _IORequest(position, count)));
            }
        }

        private void StreamOp(_IOTransfer request, Stream stream, Action<_IOTransfer, Stream> action)
        {
            action(request, stream);
        }

        protected virtual void StreamRead(_IOTransfer request, Stream stream)
        {
            try
            {
                stream.Read(request.Buffer, request.Offset, request.Count);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual void StreamWrite(_IOTransfer request, Stream stream)
        {
            stream.Write(request.Buffer, request.Offset, request.Count);
        }

        #endregion Stream Utilites

        #region Disposing

        protected override void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                }

                m_Disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion Disposing

        #region IDictionary<long,Stream> Members

        public virtual void Add(long key, Stream value)
        {
            /* Do an sanity check on possible overlaps */
            foreach (var entry in m_StreamLUT)
            {
                if (key >= entry.Key && key < entry.Key + entry.Value.Length)
                {
                    throw new ArgumentException("Stream overlaps another stream");
                }
            }

            m_StreamLUT.Add(key, value);
        }

        public bool TryGetValue(long key, out Stream value)
        {
            return m_StreamLUT.TryGetValue(key, out value);
        }

        ICollection<Stream> IDictionary<long, Stream>.Values
        {
            get { return m_StreamLUT.Values; }
        }

        Stream IDictionary<long, Stream>.this[long key]
        {
            get
            {
                return m_StreamLUT[key];
            }
            set
            {
                m_StreamLUT[key] = value;
            }
        }

        public bool ContainsKey(long key)
        {
            return m_StreamLUT.ContainsKey(key);
        }

        public ICollection<long> Keys
        {
            get { return m_StreamLUT.Keys; }
        }

        public virtual bool Remove(long key)
        {
            m_OpCompiler.ClearCache();
            return m_StreamLUT.Remove(key);
        }

        #endregion IDictionary<long,Stream> Members

        #region ICollection<KeyValuePair<long,Stream>> Members

        public virtual void Add(KeyValuePair<long, Stream> item)
        {
            m_StreamLUT.Add(item);
        }

        public bool Contains(KeyValuePair<long, Stream> item)
        {
            return m_StreamLUT.Contains(item);
        }

        public void CopyTo(KeyValuePair<long, Stream>[] array, int arrayIndex)
        {
            m_StreamLUT.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(KeyValuePair<long, Stream> item)
        {
            return m_StreamLUT.Remove(item);
        }

        public void Clear()
        {
            lock (m_IOLock)
            {
                m_OpCompiler.ClearCache();
                m_Position = 0;
                m_StreamLUT.Clear();
            }
        }

        public int Count
        {
            get { return m_StreamLUT.Count; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection<KeyValuePair<long,Stream>> Members

        #region IEnumerable<KeyValuePair<long,Stream>> Members

        IEnumerator<KeyValuePair<long, Stream>> IEnumerable<KeyValuePair<long, Stream>>.GetEnumerator()
        {
            return m_StreamLUT.GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<long,Stream>> Members

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_StreamLUT.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region IControllableStream Members

        public Boolean UseCompiler
        {
            get { return m_UseCompiler; }
            set { m_UseCompiler = value; }
        }

        #endregion IControllableStream Members

        #region IReadOnlyDictionary<long,string> Members

        IEnumerable<long> IReadOnlyDictionary<long, string>.Keys
        {
            get { return Keys; }
        }

        public bool TryGetValue(long key, out string value)
        {
            Stream stream = null;

            if (TryGetValue(key, out stream))
            {
                value = stream.ToString();
                return true;
            }

            value = null;
            return false;
        }

        IEnumerable<string> IReadOnlyDictionary<long, string>.Values
        {
            get { return ((IDictionary<Int64, Stream>)this).Values.Select((s) => s.ToString()); }
        }

        string IReadOnlyDictionary<long, string>.this[long key]
        {
            get
            {
                String name = null;

                if (TryGetValue(key, out name))
                {
                    return name;
                }

                return null;
            }
        }

        #endregion IReadOnlyDictionary<long,string> Members

        #region IEnumerable<KeyValuePair<long,string>> Members

        IEnumerator<KeyValuePair<long, string>> IEnumerable<KeyValuePair<long, string>>.GetEnumerator()
        {
            foreach (var key in Keys)
            {
                yield return new KeyValuePair<Int64, String>(key, ((IReadOnlyDictionary<long, string>)this)[key]);
            }
        }

        #endregion IEnumerable<KeyValuePair<long,string>> Members
    }
}