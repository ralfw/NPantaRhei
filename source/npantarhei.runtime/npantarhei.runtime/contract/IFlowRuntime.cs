using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
    public interface IFlowRuntime : IDisposable
	{
		void Process(IMessage message);

        void AddMessageHandler(Action<IMessage> messagehandler);
        void AddResultHandler(Action<IMessage> resulthandler);
        void AddExceptionHandler(Action<FlowRuntimeException> exceptionhandler);
		
		void AddStream(IStream stream);
		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);
		
		void Start();
		void Stop();
	}
}

