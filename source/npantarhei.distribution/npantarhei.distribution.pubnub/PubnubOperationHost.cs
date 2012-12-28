using System;
using npantarhei.distribution.pubnub.transceivers;
using npantarhei.runtime.contract;

namespace npantarhei.distribution.pubnub
{
    public class PubnubOperationHost : IDisposable
    {
        private readonly OperationHost _operationHost;

        public PubnubOperationHost(IFlowRuntime runtime, PubnubCredentials credentials, string channel)
        {
            var transceiver = new PubnubHostTransceiver(credentials, channel);
            _operationHost = new OperationHost(runtime, transceiver, transceiver);
        }

        public void Dispose()
        {
            _operationHost.Dispose();
        }
    }
}
