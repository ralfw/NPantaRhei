using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Synchronize
    {
        [Test]
        public void Implicit_sync_context()
        {
            var frc = new FlowRuntimeConfiguration()
                .AddStream(new Stream(".in", "syncNop"))
                .AddStream(new Stream("syncNop", ".out"))

                .AddFunc<string, string>("syncNop", _ => _).MakeSync();

            using (var sut = new FlowRuntime(frc))
            {
                IMessage result = null;
                var are = new AutoResetEvent(false);
                sut.Result += _ => { result = _; are.Set(); };

                sut.Process(new Message(".in", "hello"));

                Assert.IsTrue(are.WaitOne(1000));
                Assert.AreEqual("hello", result.Data.ToString());
            }
        }
    }
}
