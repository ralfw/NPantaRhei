using System;
using npantarhei.distribution.wcf.transceivers;
using npantarhei.runtime.contract;

namespace npantarhei.distribution.wcf
{
    public class WcfOperationHost : IDisposable
    {
        private readonly OperationHost _operationHost;

        public WcfOperationHost(IFlowRuntime runtime, string endpointAddress)
        {
            var transceiver = new WcfHostTransceiver(endpointAddress);
            _operationHost = new OperationHost(runtime, transceiver, transceiver);
        }

        public void Dispose()
        {
            _operationHost.Dispose();
        }
    }

}
