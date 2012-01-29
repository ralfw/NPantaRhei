using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.operations
{
	internal class Enqueue_message_for_processing
	{
		private NotifyingSingleQueue<IMessage> _messages;
		
		public void Inject(NotifyingSingleQueue<IMessage> messages)
		{
			_messages = messages;
		}
		
		public void Process(IMessage message)
		{
			_messages.Enqueue(message);
		}
	}
}

