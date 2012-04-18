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
                                              ex =>
                                              {
                                                  if (task.Message.Causalities.IsEmpty)
                                                      UnhandledException(ex);
                                                  else
                                                      Catch_exception_with_causality(task, ex);
                                              });
		    }
		    catch (Exception ex)
		    {
		        var flowEx = new FlowRuntimeException(ex, task.Message);
                if (task.Message.Causalities.IsEmpty)
                    UnhandledException(flowEx);
                else
                    Catch_exception_with_causality(task, flowEx);
		    }

		}


	    private void Put_output_in_same_context_as_input(IMessage input, IMessage output)
		{
			if (input.Port.Path != output.Port.Path)
				output = new Message(input.Port.Path + "/" + output.Port.Fullname, output.Data);
		    output.Causalities = input.Causalities;
			Result(output);
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

