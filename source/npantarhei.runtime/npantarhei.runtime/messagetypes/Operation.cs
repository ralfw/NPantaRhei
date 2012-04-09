using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Operation : IOperation
	{
		public Operation(string name, OperationAdapter implementation)
		{
			this.Name = name;
			this.Implementation = implementation;
		}

		#region IOperation implementation
		public string Name { get; private set; }
		public OperationAdapter Implementation { get; private set; }
		#endregion
	}
}

