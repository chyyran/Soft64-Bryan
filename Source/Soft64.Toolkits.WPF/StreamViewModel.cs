using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Soft64.Toolkits.WPF
{
    /// <summary>
    /// ViewModel of a System.IO.Stream object
    /// </summary>
    public class StreamViewModel : DependencyObject
    {
        private WeakReference<Stream> m_Stream;

        /* Dependency Properties */

        public static readonly DependencyProperty StreamPositionProperty =
            DependencyProperty.Register("StreamPosition", typeof(Int64), typeof(StreamViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty BufferSizeProperty =
            DependencyProperty.Register("BufferSize", typeof(Int32), typeof(StreamViewModel),
            new PropertyMetadata());

        private static readonly DependencyPropertyKey DataByteCollectionPropertyKey =
            DependencyProperty.RegisterReadOnly("DataByteCollection", typeof(ObservableCollection<Byte>), typeof(StreamViewModel),
            new PropertyMetadata());

        public event EventHandler ReadFinished;

        public static readonly DependencyProperty DataByteCollectionProperty =
            DataByteCollectionPropertyKey.DependencyProperty;

        private Object m_Lock = new Object();

        public static StreamViewModel NewModelFromStream(Stream stream)
        {
            return new StreamViewModel(new WeakReference<Stream>(stream));
        }

        private StreamViewModel(WeakReference<Stream> streamWF)
        {
            m_Stream = streamWF;
            SetValue(DataByteCollectionPropertyKey, new ObservableCollection<Byte>());
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
        }

        public void Refresh()
        {
            Dispatcher.InvokeAsync(ReadBytes);
        }

        private void ReadBytes()
        {
            Stream stream;

            if (m_Stream.TryGetTarget(out stream))
            {
                if (stream != null && BufferSize > 0 && StreamPosition >= 0)
                {
                    lock (m_Lock)
                    {
                        stream.Position = StreamPosition;
                        Int32 count = DataByteCollection.Count;
                        Int32 reqSize = BufferSize;

                        Action<Int32, Byte> readAction = (index, b) =>
                        {
                            if (index < DataByteCollection.Count)
                                DataByteCollection[index] = b;
                            else
                                DataByteCollection.Add(b);
                        };

                        Task.Factory.StartNew(() =>
                        {
                            for (Int32 i = 0; i < reqSize; i++)
                            {
                                Int32 readByte = stream.ReadByte();

                                Dispatcher.BeginInvoke(readAction, DispatcherPriority.Input, i, readByte >= 0 ? (Byte)readByte : (Byte)0);
                            }

                            Dispatcher.Invoke(OnReadFinished);
                        });
                    }
                }
            }
        }

        public StreamViewModel DeepCopy()
        {
            Stream stream = null;
            m_Stream.TryGetTarget(out stream);

            if (stream == null)
            {
                throw new InvalidOperationException("Weak reference does not point a valid reference anymore");
            }
            else
            {
                return new StreamViewModel(new WeakReference<Stream>(stream));
            }
        }

        protected virtual void OnReadFinished()
        {
            var e = ReadFinished;

            if (e != null)
            {
                e(this, new EventArgs());
            }
        }

        public Stream GetSteamSource()
        {
            Stream stream = null;
            m_Stream.TryGetTarget(out stream);
            return stream;
        }

        public Int64 StreamPosition
        {
            get { return (Int64)GetValue(StreamPositionProperty); }
            set { SetValue(StreamPositionProperty, value); }
        }

        public ObservableCollection<Byte> DataByteCollection
        {
            get { return (ObservableCollection<Byte>)GetValue(DataByteCollectionProperty); }
        }

        public Int32 BufferSize
        {
            get { return (Int32)GetValue(BufferSizeProperty); }
            set { SetValue(BufferSizeProperty, value); }
        }
    }
}