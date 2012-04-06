using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
    public interface IFlowRuntime : IDisposable
	{
		void Process(IMessage message);

        event Action<IMessage> Message;
        event Action<FlowRuntimeException> UnhandledException;
        event Action<IMessage> Result;
		
		void AddStream(IStream stream);
		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);
		
		void Start();
		void Stop();
	}
}

