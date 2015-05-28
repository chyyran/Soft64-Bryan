using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace Soft64.Toolkits.WPF
{
    /// <summary>
    /// ViewModel of a System.IO.Stream object
    /// </summary>
    public class StreamViewModel : DependencyObject, INotifyPropertyChanged
    {
        private WeakReference<Stream> m_Stream;

        /* Dependency Properties */

        public static readonly DependencyProperty StreamPositionProperty =
            DependencyProperty.Register("StreamPosition", typeof(Int64), typeof(StreamViewModel),
            new PropertyMetadata());

        public static readonly DependencyProperty BufferSizeProperty =
            DependencyProperty.Register("BufferSize", typeof(Int32), typeof(StreamViewModel),
            new PropertyMetadata());

        private Byte[] m_ReadBuffer;
        private Object m_Lock = new Object();

        public static StreamViewModel NewModelFromStream(Stream stream)
        {
            return new StreamViewModel(new WeakReference<Stream>(stream));
        }

        private StreamViewModel(WeakReference<Stream> streamWF)
        {
            m_Stream = streamWF;
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
                        Byte[] buffer = new Byte[BufferSize];
                        
                        Task.Factory.StartNew(() =>
                        {
                            stream.Read(buffer, 0, buffer.Length);
                            Dispatcher.Invoke(() => 
                            {
                                ReadBuffer = buffer;
                            });
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

        public Byte[] ReadBuffer
        {
            get {
                    return m_ReadBuffer; 
            }

            private  set 
            {
                m_ReadBuffer = value;

                var e = PropertyChanged;

                if (e != null)
                    e(this, new PropertyChangedEventArgs("ReadBuffer"));
            }
        }

        public Int32 BufferSize
        {
            get { return (Int32)GetValue(BufferSizeProperty); }
            set { SetValue(BufferSizeProperty, value); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}