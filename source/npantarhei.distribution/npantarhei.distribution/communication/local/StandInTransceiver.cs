using System;
using npantarhei.distribution.contract;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.communication.local
{
    public class StandInTransceiver : IHostProxy, IStandInStub
    {
        private readonly string _standInName;
        private readonly string _hostName;
        private readonly Action<string,HostInput> _channelToHost;

        internal StandInTransceiver(string standInStandInName, string hostName, Action<string,HostInput> channelToHost)
        {
            _standInName = standInStandInName;
            _hostName = hostName;
            _channelToHost = channelToHost;
        }


        #region IHostProxy
        public void SendToHost(HostInput input)
        {
            _channelToHost(_hostName, input);
        }
        #endregion

        #region IStandInStub
        public event Action<HostOutput> ReceivedFromHost;

        public string StandInEndpointAddress
        {
            get { return _standInName; }
        }
        #endregion


        internal void ChannelFromHost(HostOutput output)
        {
            ReceivedFromHost(output);
        }
     
        public void Dispose() {}
    }
}