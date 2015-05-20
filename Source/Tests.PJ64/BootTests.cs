using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64;
using Xunit;
using Moq;
using Soft64.MipsR4300;
using Soft64.PI;
using System.IO;
using Soft64.Media;
using Soft64.Debugging;
using Soft64.MipsR4300.CP0;

namespace Tests.PJ64
{
    public sealed class BootTests
    {
        [Fact]
        public void NtscPif6101()
        {
            TestBootSequence(MockUpCartridge(RegionType.NTSC, CICKeyType.CIC_X101));
        }

        [Fact]
        public void NtscPif6102()
        {
            TestBootSequence(MockUpCartridge(RegionType.NTSC, CICKeyType.CIC_X102));
        }

        [Fact]
        public void NtscPif6103()
        {
            TestBootSequence(MockUpCartridge(RegionType.NTSC, CICKeyType.CIC_X103));
        }

        [Fact]
        public void NtscPif6105()
        {
            TestBootSequence(MockUpCartridge(RegionType.NTSC, CICKeyType.CIC_X105));
        }

        [Fact]
        public void NtscPif6106()
        {
            TestBootSequence(MockUpCartridge(RegionType.NTSC, CICKeyType.CIC_X106));
        }

        [Fact]
        public void PalPif6101()
        {
            TestBootSequence(MockUpCartridge(RegionType.PAL, CICKeyType.CIC_X101));
        }

        [Fact]
        public void PalPif6102()
        {
            TestBootSequence(MockUpCartridge(RegionType.PAL, CICKeyType.CIC_X102));
        }

        [Fact]
        public void PalPif6103()
        {
            TestBootSequence(MockUpCartridge(RegionType.PAL, CICKeyType.CIC_X103));
        }

        [Fact]
        public void PalPif6105()
        {
            TestBootSequence(MockUpCartridge(RegionType.PAL, CICKeyType.CIC_X105));
        }

        [Fact]
        public void PalPif6106()
        {
            TestBootSequence(MockUpCartridge(RegionType.PAL, CICKeyType.CIC_X106));
        }


        private Cartridge MockUpCartridge(RegionType region, CICKeyType cic)
        {
            var mockedCart = new Mock<Cartridge>();
            var mockedCartRom = new Mock<ICartRom>();
            var mockedBootRom = new Mock<IBootRom>();
            GameSerial serial = new GameSerial(new Byte[] { 0x00, 00, 00, 00, 00, 00, 00, 00});

           MemoryStream stream = new MemoryStream();
            
            for (Int32 i = 0; i < (1024 ^ 2); i++)
                stream.WriteByte(0);

            mockedBootRom.Setup<CICKeyType>(x => x.CIC).Returns(cic);
            mockedBootRom.Setup<Int32>(x => x.BootChecksum).Returns(0x00);

            mockedCartRom.Setup<PiBusSpeedConfig>(x => x.BusConfig).Returns(new PiBusSpeedConfig(0x80, 0x37, 0x12, 0x40));
            mockedCartRom.Setup<String>(x => x.Name).Returns("MockedCartridge");
            mockedCartRom.Setup<Int32>(x => x.Clockrate).Returns(60);
            mockedCartRom.Setup<Int64>(x => x.EntryPoint).Returns(0x80000000);
            mockedCartRom.Setup<GameSerial>(x => x.Serial).Returns(serial);
            mockedCartRom.Setup<Int32>(x => x.CRC1).Returns(0x00);
            mockedCartRom.Setup<Int32>(x => x.CRC2).Returns(0x00);
            mockedCartRom.Setup<Boolean>(x => x.IsHeaderOnly).Returns(false);
            mockedCartRom.Setup<IBootRom>(x => x.BootRomInformation).Returns(mockedBootRom.Object);
            mockedCartRom.Setup<RegionType>(x => x.Region).Returns(region);
            mockedCartRom.Setup<Stream>(x => x.RomStream).Returns(stream);


            mockedCart.SetupProperty<Boolean>(c => c.IsOpened, true);
            mockedCart.Setup<Stream>(c => c.PiCartridgeStream).Returns(stream);
            mockedCart.Setup<ICartRom>(c => c.RomImage).Returns(mockedCartRom.Object);

            return mockedCart.Object;
        }

        private void TestBootSequence(Cartridge cart)
        {

            Machine machine = new Machine();
            machine.SystemBootMode = BootMode.HLE_IPL_OLD;
            machine.DeviceRCP.DevicePI.MountCartridge(cart);
            Debugger debugger = new Debugger();
            debugger.SetBootBreak(true, DebuggerBootEvent.PostBoot);
            var mockedCPUEngine = new Mock<ExecutionEngine>();
            var mockedRCPEngine = new Mock<ExecutionEngine>();
            machine.DeviceCPU.Engine = mockedCPUEngine.Object;
            machine.DeviceRCP.Engine = mockedRCPEngine.Object;
            machine.Initialize();
            machine.Run();

            ExecutionState state = machine.DeviceCPU.State;

            /* CP0 Testing */
            Assert.Equal(0x0000001FUL, state.CP0Regs[CP0RegName.Random]);
            Assert.Equal(0x00005000UL, state.CP0Regs[CP0RegName.Count]);
            Assert.Equal(0x0000005CUL, state.CP0Regs[CP0RegName.Cause]);
            Assert.Equal(0x007FFFF0UL, state.CP0Regs[CP0RegName.Context]);
            Assert.Equal(0xFFFFFFFFUL, state.CP0Regs[CP0RegName.EPC]);
            Assert.Equal(0xFFFFFFFFUL, state.CP0Regs[CP0RegName.BadVAddr]);
            Assert.Equal(0xFFFFFFFFUL, state.CP0Regs[CP0RegName.ErrorEPC]);
            Assert.Equal(0x0006E463UL, state.CP0Regs[CP0RegName.Config]);
            Assert.Equal(0x34000000UL, state.CP0Regs[CP0RegName.SR]);

            /* PIF Testing */
            AssertPIFCodes(
                Cartridge.Current.RomImage.Region, 
                Cartridge.Current.RomImage.BootRomInformation.CIC,
                state);

            debugger.Resume();
            machine.Stop();
            machine.Dispose();
        }

        private void AssertPIFCodes(RegionType region, CICKeyType cic, ExecutionState state)
        {
            Assert.Equal(0x00000000A4000040L, state.PC);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[0]);
            Assert.Equal(0xFFFFFFFFA4001F0CUL, state.GPRRegs64[6]);
            Assert.Equal(0xFFFFFFFFA4001F08UL, state.GPRRegs64[7]);
            Assert.Equal(0x00000000000000C0UL, state.GPRRegs64[8]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[9]);
            Assert.Equal(0x0000000000000040UL, state.GPRRegs64[10]);
            Assert.Equal(0xFFFFFFFFA4000040UL, state.GPRRegs64[11]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[16]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[17]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[18]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[19]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[21]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[26]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[27]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[28]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[28]);
            Assert.Equal(0xFFFFFFFFA4001FF0UL, state.GPRRegs64[29]);
            Assert.Equal(0x0000000000000000UL, state.GPRRegs64[30]);

            switch (Cartridge.Current.RomImage.Region)
            {
                case RegionType.MPAL:
                case RegionType.PAL:
                    {
                        switch (Cartridge.Current.RomImage.BootRomInformation.CIC)
                        {
                            default:
                            case CICKeyType.Unknown:
                            case CICKeyType.CIC_X102:
                                {
                                    Assert.Equal(0xFFFFFFFFC0F1D859UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000002DE108EAUL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000000UL, state.GPRRegs64[24]);

                                    break;
                                }
                            case CICKeyType.CIC_X101:
                                {
                                    break;
                                }
                            case CICKeyType.CIC_X103:
                                {
                                    Assert.Equal(0xFFFFFFFFD4646273UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000001AF99984UL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000000UL, state.GPRRegs64[24]);
                                    break;
                                }
                            case CICKeyType.CIC_X105:
                                {
                                    Assert.Equal(0xFFFFFFFFDECAAAD1UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000000CF85C13UL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000002UL, state.GPRRegs64[24]);
                                    AssertEqualMemoryReadU32(0xA4001004, 0xBDA807FC);
                                    break;
                                }
                            case CICKeyType.CIC_X106:
                                {
                                    Assert.Equal(0xFFFFFFFFB04DC903UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000001AF99984UL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000002UL, state.GPRRegs64[24]);
                                    break;
                                }

                        }

                        Assert.Equal(0x0000000000000000UL, state.GPRRegs64[20]);
                        Assert.Equal(0x0000000000000006UL, state.GPRRegs64[23]);
                        Assert.Equal(0xFFFFFFFFA4001554UL, state.GPRRegs64[31]);
                        break;
                    }

                default:
                case RegionType.Unknown:
                case RegionType.NTSC:
                    {
                        switch (Cartridge.Current.RomImage.BootRomInformation.CIC)
                        {
                            default:
                            case CICKeyType.Unknown:
                            case CICKeyType.CIC_X102:
                                {
                                    Assert.Equal(0xFFFFFFFFC95973D5UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000002449A366UL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_X101:
                                {
                                    Assert.Equal(0x000000000000003FUL, state.GPRRegs64[22]);
                                    break;
                                }
                            case CICKeyType.CIC_X103:
                                {
                                    Assert.Equal(0xFFFFFFFF95315A28UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000005BACA1DFUL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_X105:
                                {
                                    AssertEqualMemoryReadU32(0xA4001004, 0x8DA807FC);
                                    Assert.Equal(0x000000005493FB9AUL, state.GPRRegs64[5]);
                                    Assert.Equal(0xFFFFFFFFC2C20384UL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_X106:
                                {
                                    Assert.Equal(0xFFFFFFFFE067221FUL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000005CD2B70FUL, state.GPRRegs64[14]);
                                    break;
                                }
                        }

                        Assert.Equal(0x0000000000000001UL, state.GPRRegs64[20]);
                        Assert.Equal(0x0000000000000000UL, state.GPRRegs64[23]);
                        Assert.Equal(0x0000000000000003UL, state.GPRRegs64[24]);
                        Assert.Equal(0xFFFFFFFFA4001550UL, state.GPRRegs64[31]);
                        break;
                    }
            }

            switch (cic)
            {
                case CICKeyType.CIC_X101:
                    {
                        Assert.Equal(0x000000000000003FUL, state.GPRRegs64[22]);
                        break;
                    }
                case CICKeyType.Unknown:
                default:
                case CICKeyType.CIC_X102:
                    {
                        Assert.Equal(0x0000000000000001UL, state.GPRRegs64[1]);
                        Assert.Equal(0x000000000EBDA536UL, state.GPRRegs64[2]);
                        Assert.Equal(0x000000000EBDA536UL, state.GPRRegs64[3]);
                        Assert.Equal(0x000000000000A536UL, state.GPRRegs64[4]);
                        Assert.Equal(0xFFFFFFFFED10D0B3UL, state.GPRRegs64[12]);
                        Assert.Equal(0x000000001402A4CCUL, state.GPRRegs64[13]);
                        Assert.Equal(0x000000003103E121UL, state.GPRRegs64[15]);
                        Assert.Equal(0x000000000000003FUL, state.GPRRegs64[22]);
                        Assert.Equal(0xFFFFFFFF9DEBB54FUL, state.GPRRegs64[25]);
                        break;
                    }
                case CICKeyType.CIC_X103:
                    {
                        Assert.Equal(0x0000000000000001UL, state.GPRRegs64[1]);
                        Assert.Equal(0x0000000049A5EE96UL, state.GPRRegs64[2]);
                        Assert.Equal(0x0000000049A5EE96UL, state.GPRRegs64[3]);
                        Assert.Equal(0x000000000000EE96UL, state.GPRRegs64[4]);
                        Assert.Equal(0xFFFFFFFFCE9DFBF7UL, state.GPRRegs64[12]);
                        Assert.Equal(0xFFFFFFFFCE9DFBF7UL, state.GPRRegs64[13]);
                        Assert.Equal(0x0000000018B63D28UL, state.GPRRegs64[15]);
                        Assert.Equal(0x0000000000000078UL, state.GPRRegs64[22]);
                        Assert.Equal(0xFFFFFFFF825B21C9UL, state.GPRRegs64[25]);
                        break;
                    }
                case CICKeyType.CIC_X105:
                    {
                        AssertEqualMemoryReadU32(0xA4001000, 0x3C0DBFC0);
                        AssertEqualMemoryReadU32(0xA4001008, 0x25AD07C0);
                        AssertEqualMemoryReadU32(0xA400100C, 0x31080080);
                        AssertEqualMemoryReadU32(0xA4001010, 0x5500FFFC);
                        AssertEqualMemoryReadU32(0xA4001014, 0x3C0DBFC0);
                        AssertEqualMemoryReadU32(0xA4001018, 0x8DA80024);
                        AssertEqualMemoryReadU32(0xA400101C, 0x3C0BB000);
                        Assert.Equal(0x0000000000000000UL, state.GPRRegs64[1]);
                        Assert.Equal(0xFFFFFFFFF58B0FBFUL, state.GPRRegs64[2]);
                        Assert.Equal(0xFFFFFFFFF58B0FBFUL, state.GPRRegs64[3]);
                        Assert.Equal(0x0000000000000FBFUL, state.GPRRegs64[4]);
                        Assert.Equal(0xFFFFFFFF9651F81EUL, state.GPRRegs64[12]);
                        Assert.Equal(0x000000002D42AAC5UL, state.GPRRegs64[13]);
                        Assert.Equal(0x0000000056584D60UL, state.GPRRegs64[15]);
                        Assert.Equal(0x0000000000000091UL, state.GPRRegs64[22]);
                        Assert.Equal(0xFFFFFFFFCDCE565FUL, state.GPRRegs64[25]);
                        break;
                    }
                case CICKeyType.CIC_X106:
                    {
                        Assert.Equal(0x0000000000000000UL, state.GPRRegs64[1]);
                        Assert.Equal(0xFFFFFFFFA95930A4UL, state.GPRRegs64[2]);
                        Assert.Equal(0xFFFFFFFFA95930A4UL, state.GPRRegs64[3]);
                        Assert.Equal(0x00000000000030A4UL, state.GPRRegs64[4]);
                        Assert.Equal(0xFFFFFFFFBCB59510UL, state.GPRRegs64[12]);
                        Assert.Equal(0xFFFFFFFFBCB59510UL, state.GPRRegs64[13]);
                        Assert.Equal(0x000000007A3C07F4UL, state.GPRRegs64[15]);
                        Assert.Equal(0x0000000000000085UL, state.GPRRegs64[22]);
                        Assert.Equal(0x00000000465E3F72UL, state.GPRRegs64[25]);
                        break;
                    }
            }

        }

        private void AssertEqualMemoryReadU32(Int64 offset, UInt32 expected)
        {
            BinaryReader reader = new BinaryReader(Machine.Current.DeviceCPU.VirtualMemoryStream);
            reader.BaseStream.Position = offset;
            Assert.Equal(expected, reader.ReadUInt32());
        }
    }
}
