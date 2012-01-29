using System;

namespace npantarhei.runtime.contract
{
	public delegate void OperationAdapter(IMessage input, Action<IMessage> outputContinuation);
	
	public interface IOperation {
		string Name {get;}
		OperationAdapter Implementation {get;}
	}
}

