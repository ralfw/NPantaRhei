using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.flows
{
	internal class Process_message
	{
		private readonly Map_message_to_input_ports _map;
		private readonly Create_task _create;
		
		public Process_message()
		{
			// Build
			_map = new Map_message_to_input_ports();
			var output = new Output_result();
			_create = new Create_task();
			var exec = new Execute_task();
			
			// Bind	
			_process += _map.Process;
		    _map.Result += _ => Message(_);
			_map.Result += output.Process;
            output.Result += _ => Result(_);
			output.Continue += _create.Process;
			_create.Result += exec.Process;
			exec.Result += _ => Continue(_);
		    exec.HandledException += _ => Continue(_);
		    exec.UnhandledException += _ => UnhandledException(_);

		    _execute += exec.Process;
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{	
			_map.Inject(streams);
			_create.Inject(operations);
		}
		
		private readonly Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }

        private readonly Action<Task> _execute;
        public void Execute(Task task) { _execute(task); }

	    public event Action<IMessage> Message;
		public event Action<IMessage> Continue;
		public event Action<IMessage> Result;
	    public event Action<FlowRuntimeException> UnhandledException;
	}
}

