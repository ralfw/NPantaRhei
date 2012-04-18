using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
	internal class Create_task
	{
		private Dictionary<string, IOperation> _operations;
		
		internal void Inject(Dictionary<string, IOperation> operations)
		{
			_operations = operations;
		}
		
		
		public void Process(IMessage message)
		{
			var operation = _operations[message.Port.OperationName];
			Result(new Task(message, operation));
		}
		
		public event Action<Task> Result;
	}
}

