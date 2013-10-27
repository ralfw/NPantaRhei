using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests
{
	[TestFixture()]
	public class test_Parallelize
	{
		[Test()]
		public void Check_if_data_is_not_retrieved_from_queue()
		{
			const int N = 200;
			const int N_THREADS = 3;
			
			var sut = new Parallelize(N_THREADS);
			
			var are = new AutoResetEvent(false);
			var results = new List<int>();
			var threads = new Dictionary<int,bool>();
			Action<IMessage> dequeue = _ => {
				lock(results)
				{
					if (!threads.ContainsKey(Thread.CurrentThread.ManagedThreadId))
						threads.Add(Thread.CurrentThread.ManagedThreadId, true);
				    var i = (int) _.Data;
					results.Add(i);
					if (results.Count == N) are.Set();
					Thread.Sleep(i % 20);
				}
			};
			
			sut.Start();
			for(var i = 1; i<=N; i++)
				sut.Process(new Message("x", i), dequeue);
			
			Assert.IsTrue(are.WaitOne(4000));
			Assert.AreEqual((N*(N+1)/2), results.Sum());
			Assert.AreEqual(N_THREADS, threads.Count);
		}
	}
}

