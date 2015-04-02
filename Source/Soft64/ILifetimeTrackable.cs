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
    public enum LifetimeState
    {
        Created, Initialized, Running, Stopped, Disposed
    }

    public enum RequestState
    {
        Initialize, Run, Stop, Dispose
    }

    public sealed class LifeStateChangedArgs : EventArgs
    {
        public LifetimeState NewState { get; private set; }

        public LifetimeState OldState { get; private set; }

        public LifeStateChangedArgs(LifetimeState newState, LifetimeState oldState)
        {
            NewState = newState;
            OldState = oldState;
        }
    }

    public interface ILifetimeTrackable : IDisposable
    {
        event EventHandler<LifeStateChangedArgs> LifetimeStateChanged;

        void Initialize();

        void Run();

        void Stop();

        LifetimeState CurrentLifeState { get; }
    }

    public static class RuntimeModelExtensions
    {
        public static Boolean CheckStateRequestInvalid(this ILifetimeTrackable lifetimeObject, RequestState request)
        {
            switch (request)
            {
                case RequestState.Initialize:
                    {
                        if (lifetimeObject.CurrentLifeState < LifetimeState.Initialized)
                        {
                            return false;
                        }

                        break;
                    }

                case RequestState.Dispose:
                    {
                        if (lifetimeObject.CurrentLifeState > LifetimeState.Created &&
                            lifetimeObject.CurrentLifeState < LifetimeState.Disposed)
                            return false;

                        break;
                    }

                case RequestState.Run:
                    {
                        if (lifetimeObject.CurrentLifeState > LifetimeState.Created &&
                            lifetimeObject.CurrentLifeState < LifetimeState.Disposed)
                        {
                            return false;
                        }

                        break;
                    }

                case RequestState.Stop:
                    {
                        if (lifetimeObject.CurrentLifeState > LifetimeState.Running &&
                            lifetimeObject.CurrentLifeState < LifetimeState.Stopped)
                        {
                            return false;
                        }

                        break;
                    }
                default: return false;
            }

            return true;
        }

        public static Exception CreateInvalidStateException(this ILifetimeTrackable trackable, RequestState invalidRequestState)
        {
            String format = "Illegal runtime state {0} requested on type {1}";
            return new InvalidOperationException(
                String.Format(format, invalidRequestState, trackable.GetType().Name));
        }

        public static void TestPropertyChange(this ILifetimeTrackable trackable, String propertyName)
        {
            if (trackable.CurrentLifeState >= LifetimeState.Initialized)
                throw new InvalidOperationException("Cannot set property after lifetime object has been initialized: " + propertyName);
        }
    }
}