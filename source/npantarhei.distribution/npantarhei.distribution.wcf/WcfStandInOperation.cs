using System;
using npantarhei.distribution.wcf.transceivers;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace npantarhei.distribution.wcf
{
    [ActiveOperation]
    public class WcfStandInOperation : AOperation, IDisposable
    {
        private readonly StandInOperation _standInOperation;

        public WcfStandInOperation(string name, string standInEndpointAddress, string remoteEndpointAddress) : base(name)
        {
            var transceiver = new WcfStandInTransceiver(standInEndpointAddress, remoteEndpointAddress);
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
