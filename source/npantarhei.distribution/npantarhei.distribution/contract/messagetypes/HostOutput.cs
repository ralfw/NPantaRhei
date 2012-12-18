using System;

namespace npantarhei.distribution.contract.messagetypes
{
    [Serializable]
    public class HostOutput
    {
        public string Portname;
        public byte[] Data;
        public Guid CorrelationId;
    }
}