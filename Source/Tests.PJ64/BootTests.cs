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
        internal Cartridge OpenTestCartridge()
        {
            VirtualCartridge cart = new VirtualCartridge(typeof(BootTests).Assembly.GetManifestResourceStream("Tests.PJ64.m64p_test_rom.v64"));
            cart.Open();
            return cart;
        }

        [Fact]
        public void TestPIFBoot()
        {
            /* TODO: Mock other regions and CIC cartridges */
            /* TODO: Make seperate method for PIF testing */

            Machine machine = new Machine();
            machine.RCP.DevicePI.MountCartridge(OpenTestCartridge());
            Debugger debugger = new Debugger();
            debugger.SetBootBreak(true, DebuggerBootEvent.PostBoot);
            var mockedCPUEngine = new Mock<ExecutionEngine>();
            var mockedRCPEngine = new Mock<ExecutionEngine>();
            machine.CPU.Engine = mockedCPUEngine.Object;
            machine.RCP.Engine = mockedRCPEngine.Object;
            machine.Initialize();
            machine.Run();

            ExecutionState state = machine.CPU.State;

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
            Assert.Equal(0x00000000A4000040L, state.PC);
            Assert.Equal(0x0000000000000000UL,state.GPRRegs64[0]);
            Assert.Equal(0xFFFFFFFFA4001F0CUL,state.GPRRegs64[6]);
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
                            case CICKeyType.CIC_NUS_6102:
                                {
                                    Assert.Equal(0xFFFFFFFFC0F1D859UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000002DE108EAUL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000000UL, state.GPRRegs64[24]);

                                    break;
                                }
                            case CICKeyType.CIC_NUS_6103:
                                {
                                    Assert.Equal(0xFFFFFFFFD4646273UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000001AF99984UL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000000UL, state.GPRRegs64[24]);
                                    break;
                                }
                            case CICKeyType.CIC_NUS_6105:
                                {
                                    Assert.Equal(0xFFFFFFFFDECAAAD1UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000000CF85C13UL, state.GPRRegs64[14]);
                                    Assert.Equal(0x0000000000000002UL, state.GPRRegs64[24]);

                                    BinaryReader reader = new BinaryReader(Machine.Current.CPU.VirtualMemoryStream);
                                    reader.BaseStream.Position = 0xA4001004;
                                    Assert.Equal(0xBDA807FC, reader.ReadUInt32());

                                    break;
                                }
                            case CICKeyType.CIC_NUS_6106:
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
                            case CICKeyType.CIC_NUS_6102:
                                {
                                    Assert.Equal(0xFFFFFFFFC95973D5UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000002449A366UL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_NUS_6101:
                                {
                                    Assert.Equal(0x000000000000003FUL, state.GPRRegs64[22]);
                                    break;
                                }
                            case CICKeyType.CIC_NUS_6103:
                                {
                                    Assert.Equal(0xFFFFFFFF95315A28UL, state.GPRRegs64[5]);
                                    Assert.Equal(0x000000005BACA1DFUL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_NUS_6105:
                                {
                                    BinaryReader reader = new BinaryReader(Machine.Current.CPU.VirtualMemoryStream);
                                    reader.BaseStream.Position = 0xA4001004;
                                    Assert.Equal(0x8DA807FC, reader.ReadUInt32());

                                    Assert.Equal(0x000000005493FB9AUL, state.GPRRegs64[5]);
                                    Assert.Equal(0xFFFFFFFFC2C20384UL, state.GPRRegs64[14]);
                                    break;
                                }
                            case CICKeyType.CIC_NUS_6106:
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


            //switch (g_Rom->CicChipID())
            //{
            //    case CIC_NUS_6101:
            //        m_Reg.m_GPR[22].DW = 0x000000000000003F;
            //        break;
            //    case CIC_NUS_8303:		//64DD CIC
            //        m_Reg.m_GPR[22].DW = 0x00000000000000DD;
            //        break;
            //    case CIC_UNKNOWN:
            //    case CIC_NUS_6102:
            //        m_Reg.m_GPR[1].DW = 0x0000000000000001;
            //        m_Reg.m_GPR[2].DW = 0x000000000EBDA536;
            //        m_Reg.m_GPR[3].DW = 0x000000000EBDA536;
            //        m_Reg.m_GPR[4].DW = 0x000000000000A536;
            //        m_Reg.m_GPR[12].DW = 0xFFFFFFFFED10D0B3;
            //        m_Reg.m_GPR[13].DW = 0x000000001402A4CC;
            //        m_Reg.m_GPR[15].DW = 0x000000003103E121;
            //        m_Reg.m_GPR[22].DW = 0x000000000000003F;
            //        m_Reg.m_GPR[25].DW = 0xFFFFFFFF9DEBB54F;
            //        break;
            //    case CIC_NUS_6103:
            //        m_Reg.m_GPR[1].DW = 0x0000000000000001;
            //        m_Reg.m_GPR[2].DW = 0x0000000049A5EE96;
            //        m_Reg.m_GPR[3].DW = 0x0000000049A5EE96;
            //        m_Reg.m_GPR[4].DW = 0x000000000000EE96;
            //        m_Reg.m_GPR[12].DW = 0xFFFFFFFFCE9DFBF7;
            //        m_Reg.m_GPR[13].DW = 0xFFFFFFFFCE9DFBF7;
            //        m_Reg.m_GPR[15].DW = 0x0000000018B63D28;
            //        m_Reg.m_GPR[22].DW = 0x0000000000000078;
            //        m_Reg.m_GPR[25].DW = 0xFFFFFFFF825B21C9;
            //        break;
            //    case CIC_NUS_6105:
            //        MMU.SW_VAddr(0xA4001000, 0x3C0DBFC0);
            //        MMU.SW_VAddr(0xA4001008, 0x25AD07C0);
            //        MMU.SW_VAddr(0xA400100C, 0x31080080);
            //        MMU.SW_VAddr(0xA4001010, 0x5500FFFC);
            //        MMU.SW_VAddr(0xA4001014, 0x3C0DBFC0);
            //        MMU.SW_VAddr(0xA4001018, 0x8DA80024);
            //        MMU.SW_VAddr(0xA400101C, 0x3C0BB000);
            //        m_Reg.m_GPR[1].DW = 0x0000000000000000;
            //        m_Reg.m_GPR[2].DW = 0xFFFFFFFFF58B0FBF;
            //        m_Reg.m_GPR[3].DW = 0xFFFFFFFFF58B0FBF;
            //        m_Reg.m_GPR[4].DW = 0x0000000000000FBF;
            //        m_Reg.m_GPR[12].DW = 0xFFFFFFFF9651F81E;
            //        m_Reg.m_GPR[13].DW = 0x000000002D42AAC5;
            //        m_Reg.m_GPR[15].DW = 0x0000000056584D60;
            //        m_Reg.m_GPR[22].DW = 0x0000000000000091;
            //        m_Reg.m_GPR[25].DW = 0xFFFFFFFFCDCE565F;
            //        break;
            //    case CIC_NUS_6106:
            //        m_Reg.m_GPR[1].DW = 0x0000000000000000;
            //        m_Reg.m_GPR[2].DW = 0xFFFFFFFFA95930A4;
            //        m_Reg.m_GPR[3].DW = 0xFFFFFFFFA95930A4;
            //        m_Reg.m_GPR[4].DW = 0x00000000000030A4;
            //        m_Reg.m_GPR[12].DW = 0xFFFFFFFFBCB59510;
            //        m_Reg.m_GPR[13].DW = 0xFFFFFFFFBCB59510;
            //        m_Reg.m_GPR[15].DW = 0x000000007A3C07F4;
            //        m_Reg.m_GPR[22].DW = 0x0000000000000085;
            //        m_Reg.m_GPR[25].DW = 0x00000000465E3F72;
            //        break;
            //}
        }
    }
}
