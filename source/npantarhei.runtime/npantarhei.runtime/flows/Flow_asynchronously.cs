using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.flows
{
	internal class Flow_asynchronously
	{
		private Asynchronize<IMessage> _async;
		private Process_message _processMessage;
		
		public Flow_asynchronously()
		{
			// Build
			_async = new Asynchronize<IMessage>();
			_processMessage = new Process_message();
			
			// Bind
			_process += _async.Enqueue;
			_async.Dequeued += _processMessage.Process;
			_processMessage.Continue += _async.Enqueue;
			_processMessage.Result += _ => Result(_);
			
			_start += _async.Start;
			_stop += _async.Stop;
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{
			_processMessage.Inject(streams, operations);
		}
		
		private Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }
		
		public event Action<IMessage> Result;
		
		private Action _start;
		public void Start() { _start(); }
		
		private Action _stop;
		public void Stop() { _stop(); }
	}
}

