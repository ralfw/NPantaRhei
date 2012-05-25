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
			var nest = new Nest_message_flow();
			
			// Bind	
			_process += _map.Process;
			_map.Result += _ => Message(_);
			_map.Result += output.Process;
			output.Result += _ => Result(_);
			output.Continue +=_create.Process;
			_create.Result += exec.Process;
			_create.UnknownOperation += nest.Process;
			exec.Result += _ => Continue(_);
			exec.HandledException += _ => Continue(_);
			exec.UnhandledException += _ => UnhandledException(_);
			nest.Result += _map.Process;

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

	internal class Nest_message_flow
	{
		public void Process(IMessage message)
		{
			if (IsFlowInputMessage(message))
			{
				// parent/flow.port => flow/.port
				var output = new Message(Port.Build(message.Port.OperationName, "", message.Port.Name), message.Data)
				{
					Causalities = message.Causalities,
					FlowStack = message.FlowStack
				};

				if (message.Port.Path != "") output.FlowStack.Push(message.Port.Path);

				Result(output);
			}
			else
			{
				// flow/.port => parent/flow.port
				var parentFlowname = "";
				if (!message.FlowStack.IsEmpty) parentFlowname = message.FlowStack.Pop();

                var output = new Message(Port.Build(parentFlowname, Create_operation_name(message), message.Port.Name), message.Data)
				{
					Causalities = message.Causalities,
					FlowStack = message.FlowStack
				};

				Result(output);
			}
		}

	    private static string Create_operation_name(IMessage message)
	    {
	        return Is_root_level_port_needed(message) ? message.Port.Path : message.Port.OperationName;
	    }

	    private static bool Is_root_level_port_needed(IMessage message)
	    {
	        return message.Port.OperationName == "";
	    }

	    private bool IsFlowInputMessage(IMessage input)
		{
			return input.Port.Path != input.Port.OperationName && input.Port.OperationName != "";
		}


		public event Action<IMessage> Result;
	}
}

