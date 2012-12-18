using System;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.contract
{
    public interface IHostStub : IDisposable
    {
        event Action<HostInput> ReceivedFromStandIn;
        string HostEndpointAddress { get; }
    }
}