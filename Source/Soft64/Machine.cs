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
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Soft64.Debugging;
using Soft64.Engines;
using Soft64.MipsR4300;
using Soft64.PIF;
using Soft64.RCP;

namespace Soft64
{
    /// <summary>
    /// The emulator core machine.
    /// </summary>
    [Serializable]
    public class Machine
    {
        /* Private Fields */
        private Boolean m_Booted = false;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private EmulatorEngine m_CurrentEngine;
        private static ExpandoObject s_Config = new ExpandoObject();
        private SychronizedStream m_N64Memory;
        private SychronizedStream m_SafeMemory;
        private Boolean m_Disposed;
        private Boolean m_DebugServiceAttached;

        /* Events */
        public event EventHandler<MachineEventNotificationArgs> MachineEventNotification;

        public Machine()
        {
            Config.Machine = new ExpandoObject();
            Config.UI = new ExpandoObject();
            Config.Machine.SystemBootMode = (Int32)BootMode.HLE_IPL;

            Current = this;
            DeviceRCP = new RcpProcessor();
            DeviceCPU = new CPUProcessor();
            DevicePIF = new PIFModule();

            m_CurrentEngine = new SimpleEngine();
            m_CurrentEngine.EngineStatusChanged += m_CurrentEngine_EngineStatusChanged;

            m_N64Memory = new SychronizedStream(DeviceRCP.PhysicalMemoryStream);
            m_SafeMemory = new SychronizedStream(DeviceRCP.PhysicalMemoryStream.GetSafeStream());
        }

        void m_CurrentEngine_EngineStatusChanged(object sender, EngineStatusChangedArgs e)
        {
            switch (e.NewStatus)
            {
                case EngineStatus.Started: OnMachineEventNotification(MachineEventType.Started); break;
                case EngineStatus.Stopped: OnMachineEventNotification(MachineEventType.Stopped); break;
                case EngineStatus.Paused: OnMachineEventNotification(MachineEventType.Paused); break;
                case EngineStatus.Resumed: OnMachineEventNotification(MachineEventType.Running); break;
                case EngineStatus.Running: OnMachineEventNotification(MachineEventType.Running); break;
                default: break;
            }
        }

        public T GetUIConfig<T>(String propName)
        {
            var d = (IDictionary<String, Object>)Config.UI;
            Object value = null;

            if (d.TryGetValue(propName, out value))
            {
                return (T)value;
            }
            else
            {
                return default(T);
            }
        }

        public void SetUIConfig<T>(String propName, T value)
        {
            ((IDictionary<String, Object>)Config.UI)[propName] = value;
        }

        internal static dynamic Config
        {
            get { return s_Config; }
        }

        public Object ExportConfig()
        {
            return CloneExpando(s_Config);
        }

        public void ImportConfig(Object config)
        {
            if (config == null)
                return;

            MergeConfig(config, Config);
        }

        private void MergeConfig(Object obj, Object config)
        {
            var list = (IDictionary<String, Object>)obj;
            var configList = (IDictionary<String, Object>)config;

            foreach (var member in list)
            {
                if (member.Value.GetType().Equals(typeof(ExpandoObject)))
                {
                    MergeConfig(member.Value, configList[member.Key]);
                }
                else
                {
                    configList[member.Key] = member.Value;
                }
            }
        }

        private Object CloneExpando(Object obj)
        {
            var d1 = (IDictionary<String, Object>)obj;
            var tmp = new ExpandoObject();
            var d = (IDictionary<String, Object>)tmp;

            foreach (var m in d1)
            {
                if (m.Value.GetType().Equals(typeof(ExpandoObject)))
                {
                    d.Add(m.Key, CloneExpando(m.Value));
                }
                else
                {
                    d.Add(m.Key, m.Value);
                }
            }

            return tmp;
        }

        public void Run()
        {
            if (m_CurrentEngine.Status == EngineStatus.Stopped)
                m_CurrentEngine.Start();
            else
                m_CurrentEngine.ResumeThreads();
        }

        public void Pause()
        {
            m_CurrentEngine.PauseThreads();
        }

        internal void Boot()
        {
            if (m_Booted)
                return;

            try
            {
                /* Initialize N64 componenets */
                DevicePIF.Initialize();
                DeviceRCP.Initialize();
                DeviceCPU.Initialize();

                logger.Trace("** Starting emulation core **");
                OnMachineEventNotification(MachineEventType.PreBooted);

                /* Initialize core comparing */
                if (Machine.Current.MipsCompareEngine != null)
                    Machine.Current.MipsCompareEngine.Init();


                logger.Trace("Booting firmware: " + SystemBootMode.GetFriendlyName());
                SoftBootManager.SetupExecutionState(SystemBootMode);

                m_Booted = true;
                OnMachineEventNotification(MachineEventType.Booted);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An exception occuring during the boot process, see inner exception for details", e);
            }
        }

        internal void Stopped()
        {
            OnMachineEventNotification(MachineEventType.Stopped);
        }

        public void Stop()
        {
            m_CurrentEngine.Stop();
        }

        public void AttachDebuggerServices()
        {
            if (!m_DebugServiceAttached)
            {
                m_CurrentEngine.PostLoopExecute += m_CurrentEngine_PostLoopExecute;
                m_DebugServiceAttached = true;
            }
        }

        void m_CurrentEngine_PostLoopExecute(object sender, EventArgs e)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    /* TODO:
                     * Dispose Controller
                     * Dispose Audio
                     * Dispose Graphics
                     */

                    //CPU.Dispose();
                    //RCP.Dispose();


                    m_Disposed = true;
                }
            }
        }

        private void OnMachineEventNotification(MachineEventType type)
        {
            var e = MachineEventNotification;

            if (e != null)
                e(this, new MachineEventNotificationArgs(type));
        }

        public BootMode SystemBootMode
        {
            get { return (BootMode)Config.Machine.SystemBootMode; }
            set { Config.Machine.SystemBootMode = (Int32)value; }
        }

        public Boolean IsRunning
        {
            get { return m_CurrentEngine.Status == EngineStatus.Running; }
        }

        public Boolean IsPaused
        {
            get { return m_CurrentEngine.Status == EngineStatus.Paused; }
        }

        public Boolean IsStopped
        {
            get { return m_CurrentEngine.Status == EngineStatus.Stopped; }
        }

        public Stream N64Memory
        {
            get { return m_N64Memory; }
        }

        public Stream N64MemorySafe
        {
            get { return m_SafeMemory; }
        }

        public static Machine Current
        {
            get;
            private set;
        }

        public RcpProcessor DeviceRCP
        {
            get;
            private set;
        }

        public CPUProcessor DeviceCPU
        {
            get;
            private set;
        }

        public PIFModule DevicePIF
        {
            get;
            private set;
        }

        // TODO: Better place for this
        public IMipsCompareEngine MipsCompareEngine
        {
            get;
            set;
        }

        /* Short handed API */
        public void CartridgeInsert(Cartridge cartridge)
        {
            DeviceRCP.DevicePI.MountCartridge(cartridge);
        }

        public void CartridgeEject()
        {
            DeviceRCP.DevicePI.ReleaseCartridge();
        }

        public void CartridgeInsertFile(String filepath)
        {
            FileStream fs = File.OpenRead(filepath);
            VirtualCartridge cart = new VirtualCartridge(fs);
            Machine.Current.DeviceRCP.DevicePI.ReleaseCartridge();
            Machine.Current.DeviceRCP.DevicePI.MountCartridge(cart);
        }
    }
}