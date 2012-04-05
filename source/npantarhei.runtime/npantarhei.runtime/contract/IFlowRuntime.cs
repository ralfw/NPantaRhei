using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
	public interface IFlowRuntime : IDisposable
	{
		void Process(IMessage message);

        void SetResultHandler(Action<IMessage> resulthandler);
		
		void AddStream(IStream stream);
		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);
		
		void Start();
		void Stop();
	}
}

