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
    public class test_ManualResetJoin
    {
        [Test]
        public void Two_input_join()
        {
            var frc = new FlowRuntimeConfiguration()
                .AddStream(new Stream(".inString", "mrj.in0"))
                .AddStream(new Stream(".inInt", "mrj.in1"))
                .AddStream(new Stream(".inReset", "mrj.reset"))
                .AddStream(new Stream("mrj", ".out"))

                .AddManualResetJoin<string, int>("mrj");

            using(var fr = new FlowRuntime(frc))
            {
                fr.UnhandledException += Console.WriteLine;

                fr.Process(new Message(".inString", "x"));
                fr.Process(new Message(".inInt", 42));

                IMessage result = null;
                Assert.IsTrue(fr.WaitForResult(500, _ => result = _));
                var tresult = (Tuple<string, int>)result.Data;
                Assert.AreEqual("x", tresult.Item1);
                Assert.AreEqual(42, tresult.Item2);

                fr.Process(new Message(".inReset", null));
                fr.Process(new Message(".inString", "y"));
                fr.Process(new Message(".inInt", 43));
                Assert.IsTrue(fr.WaitForResult(500, _ => result = _));
                tresult = (Tuple<string, int>)result.Data;
                Assert.AreEqual("y", tresult.Item1);
                Assert.AreEqual(43, tresult.Item2);
            }
        }
    }
}
