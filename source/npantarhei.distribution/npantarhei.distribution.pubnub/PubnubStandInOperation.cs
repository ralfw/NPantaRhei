using System;
using npantarhei.distribution.pubnub.transceivers;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace npantarhei.distribution.pubnub
{
    [ActiveOperation]
    public class PubnubStandInOperation : AOperation, IDisposable
    {
        private readonly StandInOperation _standInOperation;

        public PubnubStandInOperation(string name, PubnubCredentials credentials, string hostChannel) : base(name)
        {
            var transceiver = new PubnubStandInTransceiver(credentials, hostChannel);
            _standInOperation = new StandInOperation(name, transceiver, transceiver);
        }

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            _standInOperation.Implementation(input, continueWith, unhandledException);
        }

        public void Dispose()
        {
            _standInOperation.Dispose();
        }
    }
}
