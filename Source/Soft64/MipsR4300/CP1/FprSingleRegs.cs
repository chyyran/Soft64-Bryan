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

namespace Soft64.MipsR4300.CP1
{
    /// <summary>
    /// This class access each Single value inside the FPU registers. Use only in 32-bit FPU mode.
    /// </summary>
    public sealed class FprSingleRegs : IEnumerable<Single>
    {
        private _FprReg[] m_FprRegs;

        internal FprSingleRegs(_FprReg[] regs)
        {
            m_FprRegs = regs;
        }

        public Single this[Int32 index]
        {
            get { return (index & 1) == 0 ? m_FprRegs[index >> 1].F1 : m_FprRegs[index >> 1].F2; }
            set
            {
                if ((index & 1) == 0)
                {
                    m_FprRegs[index >> 1].F1 = value;
                }
                else
                {
                    m_FprRegs[index >> 1].F2 = value;
                }
            }
        }

        public IEnumerator<float> GetEnumerator()
        {
            for (Int32 i = 0; i < m_FprRegs.Length; i++)
            {
                if ((i & 1) == 0)
                    yield return m_FprRegs[i >> 1].F1;
                else
                    yield return m_FprRegs[i >> 1].F2;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}