using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.messagetypes
{
    [TestFixture]
    public class test_AsyncWrapperOperation
    {
        [Test]
        public void Unhandled_exception_is_reported()
        {
            var sut = new AsyncWrapperOperation(new Asynchronize(), new Operation("throw", (input, continueWith, unhandledException) => { throw new ApplicationException("xxx"); }));
            sut.Start();
            try
            {
                Exception ex = null;
                var are = new AutoResetEvent(false);
                sut.Implementation(new Message("p", "hello"), _ => {}, _ => { ex = _; are.Set(); });

                Assert.IsTrue(are.WaitOne(500));
                Assert.IsInstanceOf<FlowRuntimeException>(ex);
            }
            finally
            {
                sut.Stop();
            }
        }
    }
}
