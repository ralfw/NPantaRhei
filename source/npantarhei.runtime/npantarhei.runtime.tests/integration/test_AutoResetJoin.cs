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
    public class test_AutoResetJoin
    {
        [Test]
        public void Two_input_join()
        {
            using(var fr = new FlowRuntime())
            {
                var foc = new FlowOperationContainer();
                foc.AddAutoResetJoin<string, int>("arj");
                fr.AddOperations(foc.Operations);

                fr.AddStream(new Stream(".inString", "arj.in0"));
                fr.AddStream(new Stream(".inInt", "arj.in1"));
                fr.AddStream(new Stream("arj", ".out"));

                fr.Message += _ => Console.WriteLine(_.Port);

                fr.Start();

                IMessage result = null;
                var are = new AutoResetEvent(false);
                fr.Result += _ =>
                                 {
                                     result = _; 
                                     are.Set();
                                 };

                fr.Process(new Message(".inString", "x"));
                fr.Process(new Message(".inInt", 42));

                Assert.IsTrue(are.WaitOne(500));
                var tresult = (Tuple<string, int>) result.Data;
                Assert.AreEqual("x", tresult.Item1);
                Assert.AreEqual(42, tresult.Item2);
            }
        }
    }
}
