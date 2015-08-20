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
using NLog;
using Soft64.MipsR4300;

namespace Soft64
{
    public sealed class CPUProcessor : MipsR4300Core
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Action m_Setup;
        public event EventHandler DebugStep;

        public CPUProcessor()
        {
            
        }

        public override void Initialize()
        {
            if (m_Setup == null)
                m_Setup = Engine.Step;
            base.Initialize();
        }

        public void StepOnce()
        {
            m_Setup();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EnableDebugMode()
        {
            m_Setup = () =>
            {
                Engine.Step();

                var e = DebugStep;

                if (e != null)
                {
                    DebugStep(this, new EventArgs());
                }
            };
        }
    }
}