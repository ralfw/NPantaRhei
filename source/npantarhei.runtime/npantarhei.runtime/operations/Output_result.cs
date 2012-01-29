using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	internal class Output_result
	{
		public void Process(IMessage message)
		{
			if (message.Port.IsOperationPort)
				Continue(message);
			else
				Result(message);
		}
		
		public event Action<IMessage> Continue;
		public event Action<IMessage> Result;
	}
}

