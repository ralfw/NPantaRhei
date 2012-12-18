using System;
using npantarhei.distribution.contract.messagetypes;

namespace npantarhei.distribution.contract
{
    public interface IHostProxy : IDisposable
    {
        void SendToHost(HostInput input);
    }
}