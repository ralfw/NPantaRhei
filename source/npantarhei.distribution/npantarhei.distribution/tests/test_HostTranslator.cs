using System;
using NUnit.Framework;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.translators;
using npantarhei.runtime;
using npantarhei.runtime.operations;

namespace npantarhei.distribution.tests
{
    [TestFixture]
    public class test_HostTranslator
    {
        [Test]
        public void Run()
        {
            var config = new FlowRuntimeConfiguration()
                .AddEventBasedComponent("Op", new Op())
                .AddStream(".receive@op", "op.receive")
                .AddStream("op.send", ".send@op");

            using(var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
            {
                fr.Message += Console.WriteLine;

                var sut = new HostTranslator();

                sut.Translated_input += fr.Process;
                fr.Result += sut.Process_local_output;

                Tuple<string, HostOutput> result = null;
                sut.Translated_output += _ => result = _;

                var input = new HostInput { CorrelationId = Guid.NewGuid(), Data = "hello".Serialize(), Portname = "op.receive", StandInEndpointAddress = "localhost:1234"};
                sut.Process_remote_input(input);

                Assert.AreEqual("localhost:1234", result.Item1);
                Assert.AreEqual(input.CorrelationId, result.Item2.CorrelationId);
                Assert.AreEqual("<hello>".Serialize(), result.Item2.Data);
                Assert.AreEqual("op.send", result.Item2.Portname);
            }
        }


        class Op
        {
            public void Receive(string data)
            {
                Send("<" + data + ">");
            }

            public event Action<string> Send;
        }
    }
}