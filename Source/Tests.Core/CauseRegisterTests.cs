using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soft64.MipsR4300.CP0;
using Xunit;

namespace Tests.Core
{
    public sealed class CauseRegisterTests
    {
        [Fact]
        public void TestExceptionCode()
        {
            CauseRegister reg = new CauseRegister();

            reg.ExcCode = 5;
            Assert.Equal<ExceptionCode>(ExceptionCode.AddressErrorStore, reg.ExceptionType);

            reg.ExceptionType = ExceptionCode.Watch;
            Assert.Equal(23, reg.ExcCode);

            reg.ExcCode = 5;
            Assert.Equal(20U, reg.RegisterValue);

            reg.ExceptionType = ExceptionCode.AddressErrorStore;
            Assert.Equal(20U, reg.RegisterValue);
        }
    }
}
