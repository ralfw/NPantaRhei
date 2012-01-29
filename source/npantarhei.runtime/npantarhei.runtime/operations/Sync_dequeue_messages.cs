using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.operations
{
	internal class Sync_dequeue_messages
	{
		private NotifyingSingleQueue<IMessage> _messages;
		
		public void Inject(NotifyingSingleQueue<IMessage> messages)
		{
			_messages = messages;
		}
		
		public void Process()
		{
			IMessage msg = null;
			while(_messages.TryDequeue(out msg))
				NextMessage(msg);
		}
		
		public event Action<IMessage> NextMessage;
	}
}

