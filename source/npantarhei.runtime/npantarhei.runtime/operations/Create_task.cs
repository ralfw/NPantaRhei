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
		    var normalizedOpName = message.Port.OperationName.ToLower();

		    IOperation operation;
            if (_operations.TryGetValue(normalizedOpName, out operation))
                Result(new Task(message, operation));
            else
                UnknownOperation(message);
		}
		
		public event Action<Task> Result;
	    public event Action<IMessage> UnknownOperation;
	}
}

