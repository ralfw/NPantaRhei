using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;
using npantarhei.runtime.operations;

namespace npantarhei.runtime.flows
{
	internal class Process_message
	{
		private Map_message_to_input_ports _map;
		private Create_task _create;
		
		public Process_message()
		{
			// Build
			_map = new Map_message_to_input_ports();
			var output = new Output_result();
			_create = new Create_task();
			var exec = new Execute_task();
			
			// Bind	
			_process += _map.Process;
			_map.Result += output.Process;
			output.Continue += _create.Process;
			output.Result += _ => Result(_);
			_create.Result += exec.Process;
			exec.Result += _ => Continue(_);
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{	
			_map.Inject(streams);
			_create.Inject(operations);
		}
		
		private Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }
		
		public event Action<IMessage> Continue;
		public event Action<IMessage> Result;
	}
}

