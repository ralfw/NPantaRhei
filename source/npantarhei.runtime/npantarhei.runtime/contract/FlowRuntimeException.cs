using System;

namespace npantarhei.runtime.contract
{
    public class FlowRuntimeException : ApplicationException
    {
        public FlowRuntimeException(Exception exception, IMessage context) : this("Unhandled exception during operation execution.", exception, context) {}
        public FlowRuntimeException(string message, Exception exception, IMessage context) : base(message, exception)
        {
            Context = context;
        }

        public IMessage Context { get; private set; }
    }
}