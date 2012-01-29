using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
	internal class Execute_task
	{
		public void Process(Task task)
		{
			task.Operation.Implementation(task.Message, output => Put_output_in_same_context_as_input(task.Message, output));
		}
		
		private void Put_output_in_same_context_as_input(IMessage input, IMessage output)
		{
			if (input.Port.Path != output.Port.Path)
				output = new Message(input.Port.Path + "/" + output.Port.Fullname, output.Data);
			Result(output);
		}
		
		public event Action<IMessage> Result;
	}
}

