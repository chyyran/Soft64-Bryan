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
using System.ComponentModel;
using System.Threading.Tasks;
using NLog;
using Soft64.PIF;
using Soft64.RCP;

namespace Soft64
{
    public class Machine : IRuntimeModel
    {
        private Boolean m_Booted = false;
        private BootMode m_SysBootMode = BootMode.HLE_IPL;

        private RuntimeState m_RunState =
            RuntimeState.Created;

        private static Logger logger =
            LogManager.GetCurrentClassLogger();

        private Boolean m_RunWithDebugger = false;

        public event EventHandler<RuntimeStateChangedArgs> RuntimeStateChanged;

        //private VMTaskScheduler m_VMTaskScheduler =
        //    new VMTaskScheduler();

        public event PropertyChangedEventHandler PropertyChanged;

        public Machine()
        {
            Current = this;
            RCP = new RcpProcessor();
            CPU = new CPUProcessor();
            PIF = new PIFModule();
        }

        public void Initialize()
        {
            if (this.CheckStateRequestInvalid(RequestRunState.Initialize))
                throw new MachineException("Failed to call Initialize on machine instance");

            logger.Trace("Initializing machine");

            try
            {
                /* Initialize the PIF module */
                PIF.Initialize();

                /* Initialize the RCP Processor */
                RCP.Initialize();

                /* Connect memory to the CPU Processor and initialize it */
                CPU.SetMemoryBus(Machine.Current.RCP.N64Memory);
                CPU.Initialize();

                /* TODO: Initializaitons
                 * Graphics Plugin
                 * Audio Plugin
                 * Controller Plugin
                 * ----------------------------*/

                /* Setup the task scheduler */
                //m_VMTaskScheduler.Initialize();

                m_RunState = RuntimeState.Initialized;
            }
            catch (Exception e)
            {
                throw new MachineException("Exception occurred during Initialization, see inner exception for details.", e);
            }
        }

        protected void SetNewRuntimeState(RuntimeState newState)
        {
            OnRuntimeStateChanged(newState, CurrentRuntimeState);
            m_RunState = newState;
        }

        public void Run()
        {
            if (this.CheckStateRequestInvalid(RequestRunState.Run))
                throw new MachineException("Failed to call Run on machine instance");

            logger.Trace("Running machine");

            try
            {
                if (!m_Booted)
                {
                    /* First invok the boot loader if first time running */
                    logger.Trace("Running bootloader");
                    BootMachine();
                    m_Booted = true;
                }

                logger.Trace("Starting system threads... ");

                /* TODO: Do a global backend initialize, to get video, and audio ready */

                CPU.Run();

                logger.Trace("Machine is now running ... ");

                SetNewRuntimeState(RuntimeState.Running);
            }
            catch (Exception e)
            {
                throw new MachineException("Exception occurred during Run, see inner exception details.", e);
            }
        }

        public Task<bool> Stop()
        {
            /* TODO: Stop order
             * Controller
             * Audio
             * Graphics
             * CPU
             * RCP */

            SetNewRuntimeState(RuntimeState.Stopped);

            /* TODO: Implement */
            return new Task<bool>(() => false);
        }

        public RuntimeState CurrentRuntimeState
        {
            get { return m_RunState; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (this.CheckStateRequestInvalid(RequestRunState.Dispose))
                return;

            if (disposing)
            {
                /* TODO:
                 * Dispose Controller
                 * Dispose Audio
                 * Dispose Graphics
                 */

                //CPU.Dispose();
                //RCP.Dispose();
            }

            SetNewRuntimeState(RuntimeState.Disposed);
        }

        protected virtual void OnProperyChanged(String propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }

        public BootMode SystemBootType
        {
            get { return m_SysBootMode; }
            set { m_SysBootMode = value; }
        }

        public Boolean IsRunning
        {
            get { return CurrentRuntimeState == RuntimeState.Running; }
        }

        public Boolean IsStopped
        {
            get { return CurrentRuntimeState == RuntimeState.Stopped; }
        }

        public Boolean StartWithDebugger
        {
            get { return m_RunWithDebugger; }
            set { m_RunWithDebugger = value; }
        }

        public Debugger CurrentDebugger
        {
            get;
            set;
        }

        public static Machine Current
        {
            get;
            private set;
        }

        public RcpProcessor RCP
        {
            get;
            private set;
        }

        public CPUProcessor CPU
        {
            get;
            private set;
        }

        public PIFModule PIF
        {
            get;
            private set;
        }

        private void BootMachine()
        {
            try
            {
                /* Use the boot manager to propertly setup the software state on the processors */
                if (m_SysBootMode == BootMode.MIPS_ELF)
                {
                    throw new NotSupportedException("ELFs are not supported yet.");
                }
                else
                {
                    logger.Trace("Booting from PIF: Flag=" + m_SysBootMode.ToString());
                }

                SoftBootManager.SetupExecutionState(m_SysBootMode);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An exception occuring during the boot process, see inner exception for details", e);
            }
        }

        private void ThrowOnIllegalModifiy()
        {
            if (CurrentRuntimeState > RuntimeState.Created)
                throw new MachineException("Cannot modify this property state of machine after initialization");
        }

        protected virtual void OnRuntimeStateChanged(RuntimeState newState, RuntimeState oldState)
        {
            var e = RuntimeStateChanged;

            if (e != null)
            {
                e(this, new RuntimeStateChangedArgs(newState, oldState));
            }
        }
    }
}