using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.tests.patterns
{
    [TestFixture]
    public class test_Synchronize
    {
        [Test]
        public void Implicit_sync_context()
        {
            using (var sut = new FlowRuntime())
            {
                sut.Start();

                sut.AddStream(new Stream(".in", "sync"));
                sut.AddStream(new Stream("sync", ".out"));

                var cont = new FlowOperationContainer();
                cont.RegisterSync("sync");
                sut.AddOperations(cont.Operations);

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
