using System;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.contract
{
    public interface IStandInStub : IDisposable
    {
        event Action<HostOutput> ReceivedFromHost;
        string StandInEndpointAddress { get; }
    }
}