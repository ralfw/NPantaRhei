using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using npantarhei.runtime;

namespace npantarhei.distribution.wcf.tests
{
    [TestFixture]
    public class test_Integration
    {
        [Test]
        public void Run()
        {
            var configServer = new FlowRuntimeConfiguration()
                                    .AddFunc<string, string>("hello", s => "hello, " + s)
                                    .AddStream(".@hello", "hello")
                                    .AddStream("hello", ".@hello");
            using (var server = new FlowRuntime(configServer))
            using (new WcfOperationHost(server, "localhost:8000"))
            {
                server.Message += Console.WriteLine;

                var configClient = new FlowRuntimeConfiguration()
                                    .AddOperation(new WcfStandInOperation("standin", "localhost:8100", "localhost:8000"))
                                    .AddStream(".in", "standin#hello")
                                    .AddStream("standin#hello", ".out");
                using (var client = new FlowRuntime(configClient))
                {
                    client.Message += Console.WriteLine;

                    client.Process(".in", "peter");

                    var result = "";
                    Assert.IsTrue(client.WaitForResult(2000, _ => result = (string)_.Data));

                    Assert.AreEqual("hello, peter", result);
                }
            }
        }
    }
}
