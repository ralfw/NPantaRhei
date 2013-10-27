using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Serialize
    {
        [Test]
        public void Run_on_separate_thread()
        {
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".in", "serNop0"))
                            .AddStream(new Stream("serNop0", "serNop1"))
                            .AddStream(new Stream("serNop1", ".out"));
            var asyncThreadIds0 = new List<long>();

            frc.AddFunc<string, string>("serNop0", _ =>
                                                    {
                                                        lock (asyncThreadIds0)
                                                        {
                                                            Console.WriteLine("serNop0: {0} on {1}", _, Thread.CurrentThread.ManagedThreadId);
                                                            asyncThreadIds0.Add(Thread.CurrentThread.ManagedThreadId);
                                                        }
                                                        return _;
                                                    }).MakeSerial("serNop0");

            var asyncThreadIds1 = new List<long>();
            frc.AddFunc<string, string>("serNop1", _ =>
                                                    {
                                                        lock (asyncThreadIds1)
                                                        {
                                                            Console.WriteLine("serNop1: {0} on {1}", _, Thread.CurrentThread.ManagedThreadId);
                                                            asyncThreadIds1.Add(Thread.CurrentThread.ManagedThreadId);
                                                        }
                                                        return _;
                                                    }).MakeSerial("serNop1");

            using (var sut = new FlowRuntime(frc, new Schedule_for_async_breadthfirst_processing()))
            {
                const int N = 5;
                var results = new List<IMessage>();
                long runtimeThreadId = 0;
                var are = new AutoResetEvent(false);
                sut.Result += _ =>
                                  {
                                      lock (results)
                                      {
                                          Console.WriteLine("res {0} on {1}", _.Data, Thread.CurrentThread.ManagedThreadId);
                                          runtimeThreadId = Thread.CurrentThread.ManagedThreadId;
                                          results.Add(_);
                                          if (results.Count == N) are.Set();
                                      }
                                  };

                for (var i = 0; i < N; i++ )
                    sut.Process(new Message(".in", "x" + i));

                Assert.IsTrue(are.WaitOne(10000));
                Assert.AreEqual(N, results.Count);
                Assert.That(results.Select(r => r.Data.ToString()).ToArray(), Is.EquivalentTo(new[]{"x0", "x1", "x2", "x3", "x4"}));
                Assert.IsFalse(asyncThreadIds0.Contains(runtimeThreadId));
                Assert.IsFalse(asyncThreadIds1.Contains(runtimeThreadId));
                Assert.AreEqual(0, asyncThreadIds0.Intersect(asyncThreadIds1).Count());
            }
        }
    }
}
