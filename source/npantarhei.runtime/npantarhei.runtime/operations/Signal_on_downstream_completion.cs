using System;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	internal class Signal_on_downstream_completion
	{
		public void Process(IMessage message)
		{
			Continue(message);
			DownstreamCompleted();
		}
		
		public event Action<IMessage> Continue;
		public event Action DownstreamCompleted;
	}
}

