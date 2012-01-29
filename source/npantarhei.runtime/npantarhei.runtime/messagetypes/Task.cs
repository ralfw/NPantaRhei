using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Task
	{
		public Task (IMessage message, IOperation operation)
		{
			this.Message = message;
			this.Operation = operation;
		}
		
		public IMessage Message {get; private set;}
		public IOperation Operation {get; private set;}
	}
}

