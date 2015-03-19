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
using System.Runtime.InteropServices;

namespace Soft64.MipsR4300.CP1
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct _FprReg
    {
        [FieldOffset(0)]
        public UInt64 U;

        [FieldOffset(0)]
        public Single F1;

        [FieldOffset(1)]
        public Single F2;

        [FieldOffset(0)]
        public Double D;
    }

    [Serializable]
    [CLSCompliant(false)]
    public sealed class FprRegisters : IEnumerable<UInt64>
    {
        private _FprReg[] m_Fpr;
        private FprSingleRegs m_FprSingle;
        private FprDoubleRegs m_FprDouble;

        public FprRegisters()
        {
            m_Fpr = new _FprReg[32];
            Clear();
            m_FprSingle = new FprSingleRegs(m_Fpr);
            m_FprDouble = new FprDoubleRegs(m_Fpr);
        }

        public UInt64 this[Int32 index]
        {
            get { return m_Fpr[index].U; }
            set { m_Fpr[index].U = value; }
        }

        public IEnumerator<UInt64> GetEnumerator()
        {
            for (Int32 i = 0; i < m_Fpr.Length; i++)
            {
                yield return m_Fpr[i].U;
            }
        }

        public void Clear()
        {
            for (Int32 i = 0; i < m_Fpr.Length; i++)
            {
                m_Fpr[i] = new _FprReg();
                m_Fpr[i].U = 0;
                m_Fpr[i].F1 = 0.0f;
                m_Fpr[i].F2 = 0.0f;
                m_Fpr[i].D = 0.0d;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public FprSingleRegs SingleRegisters
        {
            get { return m_FprSingle; }
        }

        public FprDoubleRegs DoubleRegisters
        {
            get { return m_FprDouble; }
        }
    }
}