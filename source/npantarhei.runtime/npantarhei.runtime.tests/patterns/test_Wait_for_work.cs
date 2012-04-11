using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.tests.patterns
{
	[TestFixture()]
	public class test_Wait_for_work
	{
		[Test()]
		public void Stop_sets_termination_flag()
		{
		    var messages = new NotifyingSingleQueue<int>();
            var sut = new Wait_for_work<int>(messages, () =>
                                                            {
                                                                int result;
                                                                var success = messages.TryDequeue(out result);
                                                                return new Tuple<bool, int>(success, result);
                                                            });
			
			sut._running = true;
			
			sut.Stop();
			Assert.IsFalse(sut._running);
		}
		
		
		[Test]
		public void Dequeues_until_stopped()
		{
			var messages = new NotifyingSingleQueue<string>();
			messages.Enqueue("a");
			var sut = new Wait_for_work<string>(messages, () =>
                                                            {
                                                                string result;
                                                                var success = messages.TryDequeue(out result);
                                                                return new Tuple<bool, string>(success, result);
                                                            });
			
			var results = new List<string>();
			sut.Dequeued += _ => results.Add(_);
			
			ThreadPool.QueueUserWorkItem(_ => {	sut.Dequeue_messages(() => {return sut._running; }); }, null);
			
			Thread.Sleep(1000);
			sut.Stop();
			
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("a", results[0]);
			string s;
			Assert.IsFalse(messages.TryDequeue(out s));
		}
		
		
		[Test]
		public void Dequeueing_is_started()
		{
		    var messages = new NotifyingSingleQueue<int>();
			var sut = new Wait_for_work<int>(messages, () =>
                                                            {
                                                                int result;
                                                                var success = messages.TryDequeue(out result);
                                                                return new Tuple<bool, int>(success, result);
                                                            });
			
			var are = new AutoResetEvent(false);
			sut.Start((_) => are.Set());
			
			Assert.IsTrue(are.WaitOne(500));
		}
	}
}

