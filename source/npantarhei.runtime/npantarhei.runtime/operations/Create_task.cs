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

            if (!_operations.ContainsKey(normalizedOpName)) throw new InvalidOperationException(string.Format("No task can be created for unknown operation '{0}'!", normalizedOpName));

            var operation = _operations[normalizedOpName];
			Result(new Task(message, operation));
		}
		
		public event Action<Task> Result;
	}
}

