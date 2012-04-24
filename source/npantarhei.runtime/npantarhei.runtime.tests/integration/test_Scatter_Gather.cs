using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.tests.integration
{
    [TestFixture]
    public class test_Scatter_Gather
    {
        [Test]
        public void Scatter_gather()
        {
            using(var fr = new FlowRuntime())
            {
                var foc = new FlowOperationContainer();
                foc.AddFunc<int, int>("sleep", _ =>
                                                     {
                                                         Console.WriteLine("sleep {0} on {1}", _, Thread.CurrentThread.GetHashCode());
                                                         Thread.Sleep(_);
                                                         return _;
                                                     }).MakeParallel();
                
                fr.AddOperations(foc.Operations);
                fr.AddOperation(new Scatter<int>("scatter"));
                fr.AddOperation(new Gather<int>("gather"));

                fr.AddStream(new Stream(".in", "scatter"));
                fr.AddStream(new Stream("scatter.stream", "sleep"));
                fr.AddStream(new Stream("scatter.count", "gather.count"));
                fr.AddStream(new Stream("sleep", "gather.stream"));
                fr.AddStream(new Stream("gather", ".out"));

                fr.Start();

                var list = new[] {10, 200, 100, 30, 200, 70};
                fr.Process(new Message(".in", list));

                IMessage result = null;
                Assert.IsTrue(fr.WaitForResult(1000, _ => result = _));
                Assert.That(list, Is.EquivalentTo(((List<int>)result.Data).ToArray()));
            }
        }
    }
}
