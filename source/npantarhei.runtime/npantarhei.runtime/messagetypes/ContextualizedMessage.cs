using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
    public class ContextualizedMessage : Message
    {
        public ContextualizedMessage(IPort port, object data, Guid correlationId) : base(port, data, correlationId) {}
    }
}