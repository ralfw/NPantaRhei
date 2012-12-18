using System;

namespace npantarhei.distribution.contract.messagetypes
{
    [Serializable]
    public class HostInput
    {
        public string Portname;
        public byte[] Data;
        public Guid CorrelationId;
        public string StandInEndpointAddress;
    }
}