using System;

namespace npantarhei.runtime.contract
{
    [Serializable]
    public class FlowRuntimeException : ApplicationException
    {
        public FlowRuntimeException(Exception exception, IMessage context) : this("Unhandled exception during operation execution.", exception, context) {}
        public FlowRuntimeException(string message, Exception exception, IMessage context) : base(message, exception)
        {
            Context = context;
        }

        public IMessage Context { get; private set; }
    }

    [Serializable]
    public class UnhandledFlowRuntimeException : FlowRuntimeException
    {
        public UnhandledFlowRuntimeException(FlowRuntimeException exception) : base("Unhandled exception during operation execution! Missing exception event handler or causality.", exception.InnerException, exception.Context) {}
    }
}