using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;

namespace npantarhei.runtime.flows
{
	internal class Flow_synchronously
	{
		private Enqueue_message_for_processing _enqueue;
		private Sync_dequeue_messages _dequeue;
		private Process_message _processMessage;
		
		public Flow_synchronously()
		{
			// Build
			var signal = new Signal_on_downstream_completion();
			_enqueue = new Enqueue_message_for_processing();
			_dequeue = new Sync_dequeue_messages();
			_processMessage = new Process_message();
			
			// Bind
			_process += signal.Process;
			signal.Continue += _enqueue.Process;
			signal.DownstreamCompleted += _dequeue.Process;
			_dequeue.NextMessage += _processMessage.Process;
			_processMessage.Continue += _enqueue.Process;
			_processMessage.Result += _ => Result(_);
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{
			var messages = new NotifyingSingleQueue<IMessage>();
			_enqueue.Inject(messages);
			_dequeue.Inject(messages);
			_processMessage.Inject(streams, operations);
		}
		
		private Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }
		
		public event Action<IMessage> Result;
	}
}

