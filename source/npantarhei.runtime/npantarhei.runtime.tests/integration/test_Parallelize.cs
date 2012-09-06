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
    public class test_Parallelize
    {
        [Test]
        public void Run_on_separate_thread()
        {
            var frc = new FlowRuntimeConfiguration();
            frc.AddStream(new Stream(".in", "parNop"));
            frc.AddStream(new Stream("parNop", ".out"));

            var cont = new FlowRuntimeConfiguration();

            var asyncThreadIds = new List<long>();
            cont.AddFunc<string, string>("parNop", _ =>
                                                    {
                                                        lock (asyncThreadIds)
                                                        {
                                                            Console.WriteLine("{0} on {1}", _, Thread.CurrentThread.GetHashCode());
                                                            asyncThreadIds.Add(Thread.CurrentThread.GetHashCode());
                                                        }
                                                        Thread.Sleep((DateTime.Now.Millisecond % 100 + 1) * 50);
                                                        return _;
                                                    }).MakeParallel();
            frc.AddOperations(cont.Operations);


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
                                          Console.WriteLine("res {0} on {1}", _.Data, Thread.CurrentThread.GetHashCode());
                                          runtimeThreadId = Thread.CurrentThread.GetHashCode();
                                          results.Add(_);
                                          if (results.Count == N) are.Set();
                                      }
                                  };

                for (var i = 0; i < N; i++ )
                    sut.Process(new Message(".in", "x" + i));

                Assert.IsTrue(are.WaitOne(10000));
                Assert.AreEqual(N, results.Count);
                Assert.That(results.Select(r => r.Data.ToString()).ToArray(), Is.EquivalentTo(new[]{"x0", "x1", "x2", "x3", "x4"}));
                Assert.IsFalse(asyncThreadIds.Contains(runtimeThreadId));
                Assert.IsTrue(asyncThreadIds.Distinct().Count() > 1);
            }
        }
    }
}
