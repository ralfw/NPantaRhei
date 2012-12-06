using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.operations
{
	internal class Execute_task
	{
		public void Process(Task task)
		{
			try
			{
				task.Operation.Implementation(task.Message,
											  output => Put_output_in_same_context_as_input(task, output),
											  ex => Handle_exception(task, ex));
			}
			catch (Exception ex)
			{
				Handle_exception(task, ex);
			}

		}


		private void Put_output_in_same_context_as_input(Task task, IMessage output)
		{
			if (output is ContextualizedMessage)
				Result(output);
			else
			{
				if (!(task.Operation is IFlow) && task.Message.Port.Path != output.Port.Path)
					output =
						new Message(
							Port.Build(task.Message.Port.Path, output.Port.OperationName, output.Port.InstanceNumber,
									   output.Port.Name),
							output.Data,
							task.Message.CorrelationId);

				if (task.Message.Port.InstanceNumber != "" && output.Port.InstanceNumber == "")
					output =
						new Message(
							Port.Build(output.Port.Path, output.Port.OperationName, task.Message.Port.InstanceNumber,
									   output.Port.Name),
							output.Data,
							task.Message.CorrelationId);

				if (task.Message.CorrelationId != Guid.Empty)
					output = new Message(output.Port, output.Data, task.Message.CorrelationId);

				if (!(task.Operation is IFlow)) output.FlowStack = task.Message.FlowStack;
				output.Causalities = task.Message.Causalities;

				Result(output);
			}
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
			var cMsg = new Message(c.Port, ex, task.Message.CorrelationId) { Priority = 99 };
			HandledException(cMsg);
		}

		
		public event Action<IMessage> Result;
		public event Action<IMessage> HandledException;
		public event Action<FlowRuntimeException> UnhandledException;
	}
}

