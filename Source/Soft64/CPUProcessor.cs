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
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Soft64.MipsR4300;

namespace Soft64
{
    public sealed class CPUProcessor : MipsR4300Core, IRuntimeModel
    {
        private CancellationTokenSource m_CPUCancellationTokenSource;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            m_CPUCancellationTokenSource = new CancellationTokenSource();

            Task cpuTask =
                new Task(() => ExecuteCPU(), m_CPUCancellationTokenSource.Token, TaskCreationOptions.LongRunning);

            cpuTask.Start();
        }

        public Task<bool> Stop()
        {
            throw new NotImplementedException();
        }

        public RuntimeState CurrentRuntimeState
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ExecuteCPU()
        {
            while (!m_CPUCancellationTokenSource.Token.IsCancellationRequested)
            {
                /* Prefetch: When we support the real cache */

                /* TODO: Debug check */

                /* Interrupt and service */

                /* Execute code / branch delay slot / interrupt/exception processing */

                //logger.Trace("cpu work");

                Thread.Sleep(100);
            }
        }

        #region IRuntimeModel Members

        public event EventHandler<RuntimeStateChangedArgs> RuntimeStateChanged;

        #endregion IRuntimeModel Members
    }
}