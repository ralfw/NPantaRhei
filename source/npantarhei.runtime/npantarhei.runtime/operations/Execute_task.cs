using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
	internal class Execute_task
	{
		public void Process(Task task)
		{
			try
			{
				task.Operation.Implementation(task.Message,
											  output => Put_output_in_same_context_as_input(task.Message, output),
											  ex => Handle_exception(task, ex));
			}
			catch (Exception ex)
			{
				Handle_exception(task, ex);
			}

		}


		private void Put_output_in_same_context_as_input(IMessage input, IMessage output)
		{
			if (input.Port.Path != output.Port.Path)
				output = new Message(input.Port.Path + "/" + output.Port.Fullname, output.Data);
			output.Causalities = input.Causalities;
			Result(output);
		}

		private void Handle_exception(Task task, Exception ex)
		{
			var exFlow = ex as FlowRuntimeException ?? new FlowRuntimeException(ex, task.Message);

			if (task.Message.Causalities.IsEmpty)
				UnhandledException(exFlow);
			else
				Catch_exception_with_causality(task, exFlow);
		}

		private void Catch_exception_with_causality(Task task, FlowRuntimeException ex)
		{
			var c = task.Message.Causalities.Peek();
			var cMsg = new Message(c.Port, ex);
			HandledException(cMsg);
		}

		
		public event Action<IMessage> Result;
		public event Action<IMessage> HandledException;
		public event Action<FlowRuntimeException> UnhandledException;
	}
}

