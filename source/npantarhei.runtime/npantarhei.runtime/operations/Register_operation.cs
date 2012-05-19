using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	internal class Register_operation
	{
		private Dictionary<string, IOperation> _operations;
		
		internal void Inject(Dictionary<string, IOperation> operations)
		{
			_operations = operations;
		}
		
		public void Process(IOperation operation)
		{
			_operations.Add(operation.Name.ToLower(), operation);
		}
	}
}

