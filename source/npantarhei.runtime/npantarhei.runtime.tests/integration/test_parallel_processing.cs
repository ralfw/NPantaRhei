using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using NUnit.Framework;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime;

namespace npantarhei.runtime.tests
{
	[TestFixture]
	public class test_parallel_processing
	{
		[Test, Explicit]
		public void Process_messages_on_different_threads()
		{
			Console.WriteLine("test thread: {0}", Thread.CurrentThread.GetHashCode());

            using (var sut = new FlowRuntime())
            {
                sut.AddStream(new Stream(".in", "doParallel**"));
                sut.AddStream(new Stream("doParallel", ".out"));

                var container = new FlowOperationContainer();

                var threads = new Dictionary<long, int>();
                container.AddFunc<int, int>("doParallel",
                                                     x =>
                                                         {
                                                             lock (threads)
                                                             {
                                                                 if (
                                                                     threads.ContainsKey(
                                                                         Thread.CurrentThread.GetHashCode()))
                                                                     threads[Thread.CurrentThread.GetHashCode()] += 1;
                                                                 else
                                                                     threads.Add(Thread.CurrentThread.GetHashCode(), 1);
                                                             }
                                                             Console.WriteLine("thread {0}: {1}.",
                                                                               Thread.CurrentThread.GetHashCode(), x);
                                                             Thread.Sleep((DateTime.Now.Millisecond%100 + 1)*50);
                                                             return x;
                                                         });
                sut.AddOperations(container.Operations);

                var are = new AutoResetEvent(false);
                var results = new List<int>();
                sut.Result += _ =>
                                {
                                    Console.WriteLine("result: {0}.", _.Data);
                                    lock (results)
                                    {
                                        results.Add((int) _.Data);
                                        if (results.Count == 5) are.Set();
                                    }
                                };

                sut.Process(new Message(".in", 1));
                sut.Process(new Message(".in", 2));
                sut.Process(new Message(".in", 3));
                sut.Process(new Message(".in", 4));
                sut.Process(new Message(".in", 5));

                Assert.IsTrue(are.WaitOne(10000), "Processing took too long; not enough numbers received");
                Assert.AreEqual(15, results.Sum(), "Wrong sum; some number got processed twice");
            }
		}
	}
}

