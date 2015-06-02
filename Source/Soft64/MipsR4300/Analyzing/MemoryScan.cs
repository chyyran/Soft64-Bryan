using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300.Analyzing
{
    public sealed class MemoryScan
    {
        private Object m_Lock = new object();

        public Task RunScan()
        {
            Task scanTask = new Task(ScanMemory);
            scanTask.Start();
            return scanTask;
        }

        private void ScanMemory()
        {
            //Int32 stride = 8;
            //Int32 length = (Int32)(0xFFFFFFFFU / stride);

            //Stream memory = Machine.Current.DeviceRCP.SafeN64Memory;
            //BinaryReader reader = new BinaryReader(memory);

            //var results = from index in Enumerable.Range(0, length)
            //              let address = index * stride
            //              let value = ReadValue(reader, address)
            //              where value >= 0
            //              select new { Address = address, Value = value };

            ///* Group sequantial results */

        }

        private UInt64 ReadValue(BinaryReader reader, Int64 position)
        {
            reader.BaseStream.Position = position;
            return reader.ReadUInt64();
        }
    }
}
