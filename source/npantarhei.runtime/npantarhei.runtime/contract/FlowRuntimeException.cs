using System;

namespace npantarhei.runtime.contract
{
    public class FlowRuntimeException : ApplicationException
    {
        public FlowRuntimeException(Exception exception, IMessage context) : base("Unhandled exception during operation execution.", exception)
        {
            Context = context;
        }

        public IMessage Context { get; private set; }
    }
}