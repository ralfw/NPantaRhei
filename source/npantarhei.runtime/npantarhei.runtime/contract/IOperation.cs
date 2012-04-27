using System;

namespace npantarhei.runtime.contract
{
    public delegate void OperationAdapter(IMessage input, Action<IMessage> outputContinuation, Action<FlowRuntimeException> unhandledException);
	
    public interface IOperation {
	    string Name {get;}
	    OperationAdapter Implementation {get;}
    }
}

