using System;
using NUnit.Framework;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.translators;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.patterns;

namespace npantarhei.distribution.tests
{
    [TestFixture]
    public class test_StandInTranslator
    {
        [Test]
        public void Run()
        {
            var config = new FlowRuntimeConfiguration()
                .AddOperation(new StandIn())
                .AddStream(".in", "standin#remoteop.in")
                .AddStream("standin#remoteop.in-out", ".out");
            using(var fr = new FlowRuntime(config, new Schedule_for_sync_depthfirst_processing()))
            {
                IMessage result = null;
                fr.Result += _ => result = _;
                fr.Message += Console.WriteLine;

                var corrId = Guid.NewGuid();
                fr.Process(new Message(".in", "hello", corrId));

                Assert.AreEqual("<hello>", result.Data);
                Assert.AreEqual(corrId, result.CorrelationId);
            }
        }


        [ActiveOperation]
        class StandIn : AOperation
        {
            private Action<IMessage> _continueWith;
            readonly StandInTranslator _sut = new StandInTranslator("localhost:1234");

            public StandIn() : base("StandIn") // use as StandIn#RemoteOp in flow
            {
                _sut.Translated_output += hi => ProcessOnHost(hi, _sut.Process_remote_input);
                _sut.Translated_input += _ => _continueWith(_);
            }

            protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
            {
                if (input is ActivationMessage)
                    _continueWith = continueWith;
                else
                    _sut.Process_local_output(input);
            }

            void ProcessOnHost(HostInput input, Action<HostOutput> sendOutput)
            {
                Assert.AreEqual("localhost:1234", input.StandInEndpointAddress);

                var inputPort = new Port(input.Portname); // remoteOp.inputPort
                var outputPortname = string.Format(inputPort.OperationName + "." + inputPort.Name + "-out"); // remoteOp.outputPort
                var output = new HostOutput { CorrelationId = input.CorrelationId, Data = ("<" + input.Data.Deserialize() + ">").Serialize(), Portname = outputPortname};
                sendOutput(output);
            }
        }
    }
}