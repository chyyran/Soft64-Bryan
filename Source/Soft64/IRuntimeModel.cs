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
using System.Threading.Tasks;

namespace Soft64
{
    public enum RuntimeState
    {
        Created, Initialized, Running, Stopped, Disposed
    }

    public enum RequestRunState
    {
        Initialize, Run, Stop, Dispose
    }

    public sealed class RuntimeStateChangedArgs : EventArgs
    {
        public RuntimeState NewState { get; private set; }

        public RuntimeState OldState { get; private set; }

        public RuntimeStateChangedArgs(RuntimeState newState, RuntimeState oldState)
        {
            NewState = newState;
            OldState = oldState;
        }
    }

    public interface IRuntimeModel : IDisposable
    {
        event EventHandler<RuntimeStateChangedArgs> RuntimeStateChanged;

        void Initialize();

        void Run();

        Task<Boolean> Stop();

        RuntimeState CurrentRuntimeState { get; }

        //void AppendMetrics(ICollection<MetricCounter> metricCollection);

        //void SetDebugger(IDebugger currentDebugger);
    }

    public static class RuntimeModelExtensions
    {
        public static Boolean CheckStateRequestInvalid(this IRuntimeModel runtimeModel, RequestRunState request)
        {
            switch (request)
            {
                case RequestRunState.Initialize:
                    {
                        if (runtimeModel.CurrentRuntimeState < RuntimeState.Initialized)
                        {
                            return false;
                        }

                        break;
                    }

                case RequestRunState.Dispose:
                    {
                        if (runtimeModel.CurrentRuntimeState > RuntimeState.Created &&
                            runtimeModel.CurrentRuntimeState < RuntimeState.Disposed)
                            return false;

                        break;
                    }

                case RequestRunState.Run:
                    {
                        if (runtimeModel.CurrentRuntimeState > RuntimeState.Created &&
                            runtimeModel.CurrentRuntimeState < RuntimeState.Disposed)
                        {
                            return false;
                        }

                        break;
                    }

                case RequestRunState.Stop:
                    {
                        if (runtimeModel.CurrentRuntimeState > RuntimeState.Running &&
                            runtimeModel.CurrentRuntimeState < RuntimeState.Stopped)
                        {
                            return false;
                        }

                        break;
                    }
                default: return false;
            }

            return true;
        }

        public static Exception CreateInvalidStateException(this IRuntimeModel runtimeModel, RequestRunState invalidRequestState)
        {
            String format = "Illegal runtime state {0} requested on type {1}";
            return new InvalidOperationException(
                String.Format(format, invalidRequestState, runtimeModel.GetType().Name));
        }
    }
}