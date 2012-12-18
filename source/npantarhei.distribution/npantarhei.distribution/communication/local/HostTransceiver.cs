using System;
using npantarhei.distribution.contract;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.communication.local
{
    public class HostTransceiver : IHostStub, IStandInProxy
    {
        private readonly string _name;
        private readonly Action<string, HostOutput> _channelToStandIn;

        internal HostTransceiver(string name, Action<string, HostOutput> channelToStandIn)
        {
            _name = name;
            _channelToStandIn = channelToStandIn;
        }


        #region IStandInProxy
        public void SendToStandIn(Tuple<string, HostOutput> output)
        {
            _channelToStandIn(output.Item1, output.Item2);
        }
        #endregion

        #region IHostStub
        public event Action<HostInput> ReceivedFromStandIn;

        public string HostEndpointAddress
        {
            get { return _name; }
        }
        #endregion


        internal void ChannelFromStandIn(HostInput input)
        {
            ReceivedFromStandIn(input);
        }


        public void Dispose() {}
    }
}