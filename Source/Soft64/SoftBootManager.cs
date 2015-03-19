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
using System.IO;
using NLog;
using Soft64.HLE_OS;
using Soft64.MipsR4300;
using Soft64.RCP;

namespace Soft64
{
    public enum BootMode
    {
        MIPS_ELF,
        HLE_IPL,
        IPL_ROM,
    }

    public static class BootModeExtensions
    {
        public static String GetFriendlyName(this BootMode bootMode)
        {
            switch (bootMode)
            {
                case BootMode.MIPS_ELF: return "MIPS Executable File";
                case BootMode.HLE_IPL: return "High-Level PIF Bootstrap";
                case BootMode.IPL_ROM: return "Real PIF Bootstrap";
                default: return "Unknown Bootmode!";
            }
        }
    }

    public static class SoftBootManager
    {
        private static Stream s_ElfStream;
        private static String s_ElfName;
        private static HLEKernel s_Kernel = new HLEKernel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void SetupExecutionState(BootMode bootMode)
        {
            if (bootMode == BootMode.IPL_ROM)
            {
                throw new InvalidOperationException("Low-level IPL booting not supported");

                Machine.Current.RCP.State.PC = 0x1FC00000;

                /* RCP begins executing the PIF rom, both CPU/RCP need to be scheduled to run on threads */
            }
            else if (bootMode == BootMode.HLE_IPL)
            {
                if (Machine.Current.RCP.DevicePI.InsertedCartridge != null)
                {
                    SimulatePIF();
                }
                else
                {
                    logger.Warn("No cartridge inserted, skipping IPL boot");
                }

                Machine.Current.CPU.State.PC = 0xA4000040;

                logger.Debug("PC is set to: " + Machine.Current.CPU.State.PC.ToString("X8"));
            }
            else if (bootMode == BootMode.MIPS_ELF)
            {
                /* TODO: Load the elf from stream and copy into memory */
                ELFExecutable executable = new ELFExecutable(s_ElfName, s_ElfStream);
                logger.Debug("Creating fake ELF process");
                logger.Debug("ELF Entry Point: " + executable.EntryPointAddress.ToString("X16"));
                s_Kernel.CreateProcess(executable);
            }
            else
            {
                throw new ArgumentException("Unknown bootmode");
            }
        }

        private static void SimulatePIF()
        {
            /* This simulates the effects of the PIF Bootloader */

            /* Copy the cartridge bootstrap into SP Memory */
            logger.Debug("PIF HLE: Copying cartridge bootrom into DMEM + 0x40");
            Machine.Current.RCP.SafeN64Memory.Position = N64MemRegions.SPDMem.ToRegionAddress() + 0x40;
            Machine.Current.RCP.DevicePI.InsertedCartridge.RomImage.BootRomInformation.CopyCode(Machine.Current.RCP.SafeN64Memory);

            /* Setup execution state */
            PIFRegInit(Machine.Current.CPU.State,
                       Machine.Current.RCP,
                       Machine.Current.RCP.DevicePI.InsertedCartridge);
        }

        public static void SetElfExecutable(FileStream inStream)
        {
            s_ElfStream = inStream;
            s_ElfName = inStream.Name;
        }

        private static void PIFRegInit(ExecutionState state, RcpProcessor rcp, Cartridge cart)
        {
            logger.Debug("PIF HLE: Using hardcoded boot values");

            state.GPRRegs64[06] = 0xFFFFFFFFA4001F0C; // IMEM Address + 0xF0C
            state.GPRRegs64[07] = 0xFFFFFFFFA4001F08; // IMEM Address + 0xF08
            state.GPRRegs64[08] = 0x00000000000000C0;
            state.GPRRegs64[10] = 0x0000000000000040;
            state.GPRRegs64[11] = 0xFFFFFFFFA4000040; // DMEM Address + 0x040
            state.GPRRegs64[29] = 0xFFFFFFFFA4001FF0; // IMEM Address + 0xFF0

            switch (cart.RomImage.Region)
            {
                case RegionType.MPAL:
                case RegionType.PAL: PIFPalValues(state, rcp, cart); break;
                case RegionType.NTSC:
                default: PIFNTSCValues(state, rcp, cart); break;
            }

            logger.Debug("PIF HLE: Cart Region: " + cart.RomImage.Region.ToString());
            logger.Debug("PIF HLE: Cart CIC:    " + cart.RomImage.BootRomInformation.CIC.ToString());

            state.GPRRegs64[20] = 0x0000000000000001;
            state.GPRRegs64[24] = 0x0000000000000003;
            state.GPRRegs64[31] = 0xFFFFFFFFA4001550; // IMEM Address + 550

            switch (cart.RomImage.BootRomInformation.CIC)
            {
                case CICKeyType.CIC_NUS_6101:
                    {
                        state.GPRRegs64[22] = 0x000000000000003F; break;
                    }
                case CICKeyType.CIC_NUS_6102:
                    {
                        state.GPRRegs64[01] = 0x0000000000000001;
                        state.GPRRegs64[02] = 0x000000000EBDA536;
                        state.GPRRegs64[03] = 0x000000000EBDA536;
                        state.GPRRegs64[04] = 0x000000000000A536;
                        state.GPRRegs64[12] = 0xFFFFFFFFED10D0B3;
                        state.GPRRegs64[13] = 0x000000001402A4CC;
                        state.GPRRegs64[15] = 0x000000003103E121;
                        state.GPRRegs64[22] = 0x000000000000003F;
                        state.GPRRegs64[25] = 0xFFFFFFFF9DEBB54F; break;
                    }
                case CICKeyType.CIC_NUS_6103:
                    {
                        state.GPRRegs64[01] = 0x0000000000000001;
                        state.GPRRegs64[02] = 0x0000000049A5EE96;
                        state.GPRRegs64[03] = 0x0000000049A5EE96;
                        state.GPRRegs64[04] = 0x000000000000EE96;
                        state.GPRRegs64[12] = 0xFFFFFFFFCE9DFBF7;
                        state.GPRRegs64[13] = 0xFFFFFFFFCE9DFBF7;
                        state.GPRRegs64[15] = 0x0000000018B63D28;
                        state.GPRRegs64[22] = 0x0000000000000078;
                        state.GPRRegs64[25] = 0xFFFFFFFF825B21C9; break;
                    }
                case CICKeyType.CIC_NUS_6105:
                    {
                        rcp.N64Memory.Position = 0x04001000;
                        BinaryWriter writer = new BinaryWriter(rcp.N64Memory);

                        writer.Write(0x3C0DBFC0U); // SP_IMEM[0]
                        writer.BaseStream.Position += 4;
                        writer.Write(0x25AD07C0U); // SP_IMEM[2]
                        writer.Write(0x31080080U); // SP_IMEM[3]
                        writer.Write(0x5500FFFCU); // SP_IMEM[4]
                        writer.Write(0x3C0DBFC0U); // SP_IMEM[5]
                        writer.Write(0x8DA80024U); // SP_IMEM[6]
                        writer.Write(0x3C0BB000U); // SP_IMEM[7]
                        state.GPRRegs64[02] = 0xFFFFFFFFF58B0FBF;
                        state.GPRRegs64[03] = 0xFFFFFFFFF58B0FBF;
                        state.GPRRegs64[04] = 0x0000000000000FBF;
                        state.GPRRegs64[12] = 0xFFFFFFFF9651F81E;
                        state.GPRRegs64[13] = 0x000000002D42AAC5;
                        state.GPRRegs64[15] = 0x0000000056584D60;
                        state.GPRRegs64[22] = 0x0000000000000091;
                        state.GPRRegs64[25] = 0xFFFFFFFFCDCE565F; break;
                    }
                case CICKeyType.CIC_NUS_6106:
                    {
                        state.GPRRegs64[02] = 0xFFFFFFFFA95930A4;
                        state.GPRRegs64[03] = 0xFFFFFFFFA95930A4;
                        state.GPRRegs64[04] = 0x00000000000030A4;
                        state.GPRRegs64[12] = 0xFFFFFFFFBCB59510;
                        state.GPRRegs64[13] = 0xFFFFFFFFBCB59510;
                        state.GPRRegs64[15] = 0x000000007A3C07F4;
                        state.GPRRegs64[22] = 0x0000000000000085;
                        state.GPRRegs64[25] = 0x00000000465E3F72; break;
                    }
            }
        }

        private static void PIFNTSCValues(ExecutionState state, RcpProcessor rcp, Cartridge cart)
        {
            switch (cart.RomImage.BootRomInformation.CIC)
            {
                case CICKeyType.CIC_NUS_6102:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFFC95973D5;
                        state.GPRRegs64[14] = 0x000000002449A366; break;
                    }
                case CICKeyType.CIC_NUS_6103:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFF95315A28;
                        state.GPRRegs64[14] = 0x000000005BACA1DF; break;
                    }
                case CICKeyType.CIC_NUS_6105:
                    {
                        rcp.N64Memory.Position = 0x04001000 + 4;
                        BinaryWriter writer = new BinaryWriter(rcp.N64Memory);
                        writer.Write(0x8DA807FCU);
                        state.GPRRegs64[05] = 0x000000005493FB9A;
                        state.GPRRegs64[14] = 0xFFFFFFFFC2C20384; break;
                    }
                case CICKeyType.CIC_NUS_6106:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFFE067221F;
                        state.GPRRegs64[14] = 0x000000005CD2B70F; break;
                    }
            }

            state.GPRRegs64[20] = 0x0000000000000001;
            state.GPRRegs64[24] = 0x0000000000000003;
            state.GPRRegs64[31] = 0xFFFFFFFFA4001550;
        }

        private static void PIFPalValues(ExecutionState state, RcpProcessor rcp, Cartridge cart)
        {
            switch (cart.RomImage.BootRomInformation.CIC)
            {
                case CICKeyType.CIC_NUS_6102:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFFC0F1D859;
                        state.GPRRegs64[14] = 0x000000002DE108EA; break;
                    }
                case CICKeyType.CIC_NUS_6103:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFFD4646273;
                        state.GPRRegs64[14] = 0x000000001AF99984; break;
                    }
                case CICKeyType.CIC_NUS_6105:
                    {
                        rcp.N64Memory.Position = 0x04001000 + 4;
                        BinaryWriter writer = new BinaryWriter(rcp.N64Memory);
                        writer.Write(0xBDA807FCU);
                        state.GPRRegs64[05] = 0xFFFFFFFFDECAAAD1;
                        state.GPRRegs64[14] = 0x000000000CF85C13;
                        state.GPRRegs64[24] = 0x0000000000000002; break;
                    }
                case CICKeyType.CIC_NUS_6106:
                    {
                        state.GPRRegs64[05] = 0xFFFFFFFFB04DC903;
                        state.GPRRegs64[14] = 0x000000001AF99984;
                        state.GPRRegs64[24] = 0x0000000000000002; break;
                    }
            }

            state.GPRRegs64[23] = 0x0000000000000006;
            state.GPRRegs64[31] = 0xFFFFFFFFA4001554;
        }

        public static HLEKernel Kernel
        {
            get { return SoftBootManager.s_Kernel; }
        }
    }
}