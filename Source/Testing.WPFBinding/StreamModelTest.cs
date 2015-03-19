using System;
using System.IO;
using Soft64Binding.WPF;
using Xunit;

namespace Testing.WPFBinding
{
    public class StreamModelTest
    {
        private static MemoryStream s_TestStream;

        private static Byte[] s_Bytes = {
                              0x00, 0xDE, 0xAA, 0xEE, 0xDD
                          };

        static StreamModelTest()
        {
            s_TestStream = new MemoryStream(s_Bytes);
        }

        [Fact]
        public void TestDPs()
        {
            StreamViewModel model = new StreamViewModel(s_TestStream);
            ;
            model.SetValue(StreamViewModel.StreamPositionProperty, 0xEADL);
            model.SetValue(StreamViewModel.BufferSizeProperty, 0xBCD);

            Assert.Equal(0xEADL, model.StreamPosition);
            Assert.Equal(0xBCD, model.BufferSize);
        }

        [Fact]
        public void TestByteCollection()
        {
            StreamViewModel model = new StreamViewModel(s_TestStream);
            model.BufferSize = (Int32)s_TestStream.Length;
            //model.ReadBytes();
        }

        [Fact]
        public void TestByteCollectionAsync()
        {
            //StreamViewModel model = new StreamViewModel(s_TestStream);
            //model.BufferSize = (Int32)s_TestStream.Length;
            //var task = model.ReadBytesAsync();
            //DispatcherUtil.DoEvents();
            //task.Wait();
        }
    }
}