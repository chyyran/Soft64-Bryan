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
using System.Xml.Serialization;
using NLog;
using Soft64.Debugging;
using Soft64.Engines;
using Soft64.PIF;
using Soft64.RCP;

namespace Soft64
{
    /// <summary>
    /// The emulator core machine.
    /// </summary>
    [Serializable]
    public class Machine : ILifetimeTrackable, IConfiguration
    {
        /* Private Fields */
        private Boolean m_Booted = false;
        private LifetimeState m_RunState =LifetimeState.Created;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private EmulatorEngine m_CurrentEngine;

        /* Property Backings */
        private BootMode m_PropSysBootMode = BootMode.HLE_IPL;
        private EmulatorEngine m_PropEngine;

        public event EventHandler<LifeStateChangedArgs> LifetimeStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public Machine()
        {
            Current = this;
            RCP = new RcpProcessor();
            CPU = new CPUProcessor();
            PIF = new PIFModule();
            m_PropEngine = new SimpleEngine();
        }

        public void Initialize()
        {
            if (this.CheckStateRequestInvalid(RequestState.Initialize))
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

                /* Initilaize the runtime engine */
                m_CurrentEngine = m_PropEngine;
                m_CurrentEngine.Initialize();

                m_RunState = LifetimeState.Initialized;
            }
            catch (Exception e)
            {
                logger.Trace("Exception trace", e);

                throw new MachineException("Exception occurred during Initialization, see inner exception for details.", e);
            }
        }

        protected void SetNewRuntimeState(LifetimeState newState)
        {
            OnRuntimeStateChanged(newState, CurrentLifeState);
            m_RunState = newState;
        }

        public void Run()
        {
            if (this.CheckStateRequestInvalid(RequestState.Run))
                throw new MachineException("Failed to call Run on machine instance");

            logger.Trace("Running machine");

            try
            {
                if (!m_Booted)
                {
                    logger.Trace("Running bootloader");
                    BootMachine();
                    m_Booted = true;

                    /* TODO: Initialize multimedia backends */
                }

                logger.Trace("Starting system threads... ");
                m_CurrentEngine.Run();
                logger.Trace("Machine is now running ... ");
                SetNewRuntimeState(LifetimeState.Running);
            }
            catch (Exception e)
            {
                throw new MachineException("Exception occurred during Run, see inner exception details.", e);
            }
        }

        public void Stop()
        {
            /* TODO: Stop order
             * Controller
             * Audio
             * Graphics
             * CPU
             * RCP */

            SetNewRuntimeState(LifetimeState.Stopped);
        }
       
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (this.CheckStateRequestInvalid(RequestState.Dispose))
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

            SetNewRuntimeState(LifetimeState.Disposed);
        }

        protected virtual void OnProperyChanged(String propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
                e(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BootMachine()
        {
            try
            {
                /* Notify debugger before booting */
                if (Debugger.Current != null) Debugger.Current.NotifyBootEvent(DebuggerBootEvent.PreBoot);

                /* Use the boot manager to propertly setup the software state on the processors */
                logger.Trace("Booting: " + m_PropSysBootMode.GetFriendlyName());

                SoftBootManager.SetupExecutionState(m_PropSysBootMode);

                /* Notify debugger after booting */
                if (Debugger.Current != null) Debugger.Current.NotifyBootEvent(DebuggerBootEvent.PostBoot);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An exception occuring during the boot process, see inner exception for details", e);
            }
        }

        protected virtual void OnRuntimeStateChanged(LifetimeState newState, LifetimeState oldState)
        {
            var e = LifetimeStateChanged;

            if (e != null)
            {
                e(this, new LifeStateChangedArgs(newState, oldState));
            }
        }

        public void SaveConfig(System.Collections.Generic.IDictionary<string, object> propertyBag)
        {
            throw new NotImplementedException();
        }

        public void ReadConfig(System.Collections.Generic.IDictionary<string, object> proeprtyBag)
        {
            throw new NotImplementedException();
        }

        public LifetimeState CurrentLifeState
        {
            get { return m_RunState; }
        }

        public BootMode SystemBootMode
        {
            get { return m_PropSysBootMode; }
            set { m_PropSysBootMode = value; }
        }

        public Boolean IsRunning
        {
            get { return CurrentLifeState == LifetimeState.Running; }
        }

        public Boolean IsStopped
        {
            get { return CurrentLifeState == LifetimeState.Stopped; }
        }

        public EmulatorEngine Engine
        {
            get { return m_PropEngine; }
            set { m_PropEngine = value; }
        }

        /* Machine Components */

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
    }
}