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

namespace Soft64.MipsR4300
{
    public delegate void InterruptHandler();

    public class InterruptTable
    {
        private MipsR4300Core m_MipsCore;

        public event InterruptHandler Interrupt0;

        public event InterruptHandler Interrupt1;

        public event InterruptHandler Interrupt2;

        public event InterruptHandler Interrupt3;

        public event InterruptHandler Interrupt4;

        public event InterruptHandler Interrupt5;

        public InterruptTable(MipsR4300Core mipsCore)
        {
            m_MipsCore = mipsCore;
        }

        public void CheckInterrupts()
        {
            // Boolean[] status
            /* Status register - IM Masking
             * Cause Reg = Interrupts Pending
             * Use exception type - Bp (9) = Breakpoint
             * PIN Or Interrupt Register *
             * So implement a interrupt register within this class */
        }
    }
}