using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.flows
{
	internal class Flow_asynchronously
	{
		private readonly Process_message _processMessage;
		
		public Flow_asynchronously(IScheduler schedule)
		{
			// Build
			var throttle = new Throttle_message_flow();
			var handle_exception = new Handle_exception();
			_processMessage = new Process_message();

			// Bind
			_process += schedule.ProcessExternalMessage;
			schedule.Result += throttle.Process;
			throttle.Continue += handle_exception.Process;
			handle_exception.Continue += _processMessage.Process;
			handle_exception.UnhandledException += _ => UnhandledException(_);
			_processMessage.Message += _ => Message(_);
			_processMessage.Continue += schedule.ProcessInternalMessage;
			_processMessage.Result += _ => Result(_);
			_processMessage.UnhandledException += _ => UnhandledException(_);

			_execute += _processMessage.Execute;

			_start += schedule.Start;
			_stop += schedule.Stop;
			_throttle += throttle.Delay;
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{
			_processMessage.Inject(streams, operations);
		}
		
		private readonly Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }

		private readonly Action<Task> _execute;
		public void Execute(Task task) { _execute(task); }

		public event Action<IMessage> Message;
		public event Action<IMessage> Result;
		public event Action<FlowRuntimeException> UnhandledException;
		
		private readonly Action _start;
		public void Start() { _start(); }
		
		private readonly Action _stop;
		public void Stop() { _stop(); }

		private readonly Action<int> _throttle;
		public void Throttle(int delayMilliseconds) { _throttle(delayMilliseconds); }
	}
}

