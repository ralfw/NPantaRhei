using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	internal class Output_result
	{
		public void Process(IMessage message)
		{
			if (IsResultPort(message))
				Result(message);
			else
				Continue(message);
		}

	    private static bool IsResultPort(IMessage message)
	    {
	        return message.Port.Path == "" && !message.Port.IsOperationPort;
	    }

	    public event Action<IMessage> Continue;
		public event Action<IMessage> Result;
	}
}

