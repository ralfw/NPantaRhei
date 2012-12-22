using System.Collections.Generic;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.runtime.contract;

namespace npantarhei.distribution.communication.local
{
    public class SyncBus
    {
        private readonly Dictionary<string, StandInTransceiver> _standIns = new Dictionary<string, StandInTransceiver>();
        private readonly Dictionary<string, HostTransceiver> _hosts = new Dictionary<string, HostTransceiver>();
 

        public StandInOperation CreateStandInOperation(string name, string hostName)
        {
            var trans = RegisterStandIn(name, hostName);
            return new StandInOperation(name, trans, trans);
        }

        StandInTransceiver RegisterStandIn(string standInName, string hostName)
        {
            var trans = new StandInTransceiver(standInName, hostName, SendFromStandInToHost);
            _standIns.Add(standInName, trans);
            return trans;
        }

        
        public OperationHost CreateOperationHost(IFlowRuntime runtime, string name)
        {
            var trans = RegisterHost(name);
            return new OperationHost(runtime, trans, trans);
        }

        HostTransceiver RegisterHost(string name)
        {
            var trans = new HostTransceiver(name, SendFromHostToStandIn);
            _hosts.Add(name, trans);
            return trans;
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
