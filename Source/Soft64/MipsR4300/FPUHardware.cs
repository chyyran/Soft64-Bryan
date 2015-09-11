using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Soft64.MipsR4300
{
    public static class FPUHardware
    {
        private const Int32 _MCW_RC = 0x00000300;
        private const Int32 _MCW_EM = 0x0008001f;
        private const Int32 _EM_INEXACT = 0x00000001;
        private const Int32 _EM_UNDERFLOW = 0x00000002;
        private const Int32 _EM_OVERFLOW = 0x00000004;
        private const Int32 _EM_ZERODIVIDE = 0x00000008;
        private const Int32 _EM_INVALID = 0x00000010;

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _controlfp(int newControl, int mask);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _statusfp();

        public static void SetRoundingMode(FPURoundMode mode)
        {
            _controlfp((Int32)mode, _MCW_RC);
        }

        public static void ClearFPUExceptionMask()
        {
            _controlfp(0, _MCW_EM);
        }

        public static Boolean CheckFPUException()
        {
            return _statusfp() != 0;
        }

        public static FPUExceptionType GetFPUException()
        {
            Int32 status = _statusfp();

            if (status != 0)
            {
                if ((status & _EM_INEXACT) == _EM_INEXACT)
                {
                    return FPUExceptionType.Inexact;
                }
                else if ((status & _EM_INVALID) == _EM_INVALID)
                {
                    return FPUExceptionType.Invalid;
                }
                else if ((status & _EM_OVERFLOW) == _EM_OVERFLOW)
                {
                    return FPUExceptionType.Overflow;
                }
                else if ((status & _EM_UNDERFLOW) == _EM_UNDERFLOW)
                {
                    return FPUExceptionType.Underflow;
                }
                else if ((status & _EM_ZERODIVIDE) == _EM_ZERODIVIDE)
                {
                    return FPUExceptionType.DivideByZero;
                }
                else
                {
                    return FPUExceptionType.Unimplemented;
                }
            }
            else
            {
                return FPUExceptionType.Unimplemented;
            }
        }
    }
}
