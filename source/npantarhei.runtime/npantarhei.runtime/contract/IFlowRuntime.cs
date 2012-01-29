using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
	public interface IFlowRuntime : IDisposable
	{
		void ProcessSync(IMessage message);
		void ProcessAsync(IMessage message);
		event Action<IMessage> Result;
		
		void AddStream(IStream stream);
		void AddOperation(IOperation operation);
		void AddOperations(IEnumerable<IOperation> operations);
		
		void Start();
		void Stop();
	}
}

