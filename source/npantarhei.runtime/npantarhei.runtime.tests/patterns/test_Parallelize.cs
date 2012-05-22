using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NUnit.Framework;
using npantarhei.runtime.contract;
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
			
			var sut = new Parallelize<int>(N_THREADS);
			
			var are = new AutoResetEvent(false);
			var results = new List<int>();
			var threads = new Dictionary<long,bool>();
			Action<int> dequeue = _ => {
				lock(results)
				{
					if (!threads.ContainsKey(Thread.CurrentThread.GetHashCode()))
						threads.Add(Thread.CurrentThread.GetHashCode(), true);
					results.Add(_);
					if (results.Count == N) are.Set();
					Thread.Sleep(_ % 10);
				}
			};
			
			sut.Start();
			for(var i = 1; i<=N; i++)
				sut.Process(i, dequeue);
			
			Assert.IsTrue(are.WaitOne(4000));
			Assert.AreEqual((N*(N+1)/2), results.Sum());
			Assert.AreEqual(N_THREADS, threads.Count);
		}
	}
}

