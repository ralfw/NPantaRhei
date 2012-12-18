using System.Collections.Generic;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.communication.local
{
    public class SyncBus
    {
        private readonly Dictionary<string, StandInTransceiver> _standIns = new Dictionary<string, StandInTransceiver>();
        private readonly Dictionary<string, HostTransceiver> _hosts = new Dictionary<string, HostTransceiver>();
 
        public StandInTransceiver RegisterStandIn(string standInName, string hostName)
        {
            var sits = new StandInTransceiver(standInName, hostName, SendFromStandInToHost);
            _standIns.Add(standInName, sits);
            return sits;
        }

        public HostTransceiver RegisterHost(string name)
        {
            var hts = new HostTransceiver(name, SendFromHostToStandIn);
            _hosts.Add(name, hts);
            return hts;
        }

        void SendFromStandInToHost(string hostName, HostInput input)
        {
            _hosts[hostName].ChannelFromStandIn(input);
        }

        void SendFromHostToStandIn(string standInName, HostOutput output)
        {
            _standIns[standInName].ChannelFromHost(output);
        }
    }
}
