using System;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.contract
{
    public interface IStandInProxy : IDisposable
    {
        void SendToStandIn(Tuple<string, HostOutput> output);
    }
}
