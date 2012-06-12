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
    public class test_correlationId_handling
    {
        [Test]
        public void CorrelationId_is_kept_throughout_flow()
        {
            var config = new FlowRuntimeConfiguration()
                                .AddStreamsFrom(@"
                                                /
                                                .in, addX,
                                                addX, addY
                                                addY, .out
                                                ")
                                .AddFunc<string, string>("addX", _ => _ + "x")
                                .AddFunc<string, string>("addY", _ => _ + "y");

            using(var fr = new FlowRuntime(config))
            {
                fr.Message += Console.WriteLine;

                var corrId1 = Guid.NewGuid();
                var corrId2 = Guid.NewGuid();

                var results = new List<IMessage>();
                var are = new AutoResetEvent(false);
                fr.Result += _ =>
                                 {
                                     results.Add(_);
                                     if (results.Count == 2) are.Set();
                                 };

                fr.Process(new Message(".in", "1", corrId1));
                fr.Process(new Message(".in", "2", corrId2));

                Assert.IsTrue(are.WaitOne(1000));

                results.Sort((a, b) => ((string)a.Data).CompareTo((string)b.Data));
                Assert.AreEqual("1xy", results[0].Data);
                Assert.AreEqual("2xy", results[1].Data);
                Assert.AreEqual(corrId1, results[0].CorrelationId);
                Assert.AreEqual(corrId2, results[1].CorrelationId);
            }
        }
    }
}
