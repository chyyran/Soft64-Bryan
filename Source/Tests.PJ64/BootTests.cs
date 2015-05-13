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

            Assert.Equal(state.CP0Regs[CP0RegName.Random], 0x1FUL);
            /* TODO: */
        }
    }
}
