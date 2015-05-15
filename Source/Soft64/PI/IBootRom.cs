using System;
namespace Soft64.PI
{
    public interface IBootRom
    {
        int BootChecksum { get; }

        Soft64.CICKeyType CIC { get; }

        void CopyCode(System.IO.Stream dest);
    }
}
