using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.PI
{
    public interface ICartRom
    {
        PiBusSpeedConfig BusConfig { get; }

        String Name { get; }

        Int32 Clockrate { get; }

        Int64 EntryPoint { get; }

        GameSerial Serial { get; }

        Int32 CRC1 { get; }

        Int32 CRC2 { get; }

        Boolean IsHeaderOnly { get; }

        IBootRom BootRomInformation { get; }

        RegionType Region { get; }

        Int32 Release { get; }

        Int32 GetAIDacRate();

        Int32 GetVIRate();

        Stream RomStream { get; }
    }
}
