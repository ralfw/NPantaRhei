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
    public class test_Asynchronize
    {
        [Test]
        public void Run_on_separate_thread()
        {
            var frc = new FlowRuntimeConfiguration();
            frc.AddStream(new Stream(".in", "asyncNop"));
            frc.AddStream(new Stream("asyncNop", ".out"));

            var cont = new FlowRuntimeConfiguration();

            long asyncThreadId = 0;
            cont.AddFunc<string, string>("asyncNop", _ =>
                                                    {
                                                        asyncThreadId = Thread.CurrentThread.GetHashCode();
                                                        return _;
                                                    }).MakeAsync();
            frc.AddOperations(cont.Operations);

            using (var sut = new FlowRuntime(frc))
            {
                IMessage result = null;
                long runtimeThreadId = 0;
                var are = new AutoResetEvent(false);
                sut.Result += _ =>
                                  {
                                      runtimeThreadId = Thread.CurrentThread.GetHashCode();
                                      result = _; 
                                      are.Set();
                                  };

                sut.Process(new Message(".in", "hello"));

                Assert.IsTrue(are.WaitOne(1000));
                Assert.AreEqual("hello", result.Data.ToString());
                Assert.AreNotEqual(runtimeThreadId, asyncThreadId);
            }
        }

        [Test]
        public void Catch_exception_from_background()
        {
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".in", "throw"))
                            .AddAction<string>("throw", (string _) => { throw new ApplicationException("xxx"); }).MakeAsync();

            using (var sut = new FlowRuntime(frc))
            {
                FlowRuntimeException ex = null;
                var are = new AutoResetEvent(false);
                sut.UnhandledException += _ =>
                                                {
                                                    ex = _;
                                                    are.Set();
                                                };

                sut.Process(new Message(".in", "hello"));

                Assert.IsTrue(are.WaitOne(1000));
                Assert.AreEqual("xxx", ex.InnerException.Message);
            }
        }
    }
}
