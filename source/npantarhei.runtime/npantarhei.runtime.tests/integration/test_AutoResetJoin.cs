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
            var frc = new FlowRuntimeConfiguration()
                            .AddStream(new Stream(".inString", "arj.in0"))
                            .AddStream(new Stream(".inInt", "arj.in1"))
                            .AddStream(new Stream("arj", ".out"))

                            .AddAutoResetJoin<string, int>("arj");

            using(var fr = new FlowRuntime(frc))
            {
                fr.Message += _ => Console.WriteLine(_.Port);

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
