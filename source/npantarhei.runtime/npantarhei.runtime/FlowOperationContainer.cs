using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime
{
	public class FlowOperationContainer
	{
		private List<IOperation> _operations = new List<IOperation>();
		
		
		public void RegisterFunction<TInput, TOutput>(string name, Func<TInput, TOutput> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont) => {
										  		var result = implementation((TInput)input.Data);
												outputCont(new Message(name, result));
										  }
						   ));
		}
		
		
		public void RegisterAction<TInput>(string name, Action<TInput> implementation)
		{
			_operations.Add(new Operation(name, (input, _) => implementation((TInput)input.Data)));
		}
		
		
		public void RegisterAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont) => implementation((TInput)input.Data, 
															                    output => outputCont(new Message(name, output)))
						   ));
		}
		
		
		public IEnumerable<IOperation> Operations {	get { return _operations; }	}
	}
}

