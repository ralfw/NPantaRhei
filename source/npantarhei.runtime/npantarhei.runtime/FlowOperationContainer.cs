using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime
{
	public class FlowOperationContainer
	{
		private readonly List<IOperation> _operations = new List<IOperation>();


        public FlowOperationContainer AddFunc<TOutput>(string name, Func<TOutput> implementation)
        {
            _operations.Add(new Operation(name,
                                          (input, outputCont) => {
                                                var result = implementation();
                                                outputCont(new Message(name, result));
                                          }
                           ));
            return this;
        }

		public FlowOperationContainer AddFunc<TInput, TOutput>(string name, Func<TInput, TOutput> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont) => {
										  		var result = implementation((TInput)input.Data);
												outputCont(new Message(name, result));
										  }
						   ));
		    return this;
		}
		
		
		public FlowOperationContainer AddAction<TInput>(string name, Action<TInput> implementation)
		{
			_operations.Add(new Operation(name, (input, _) => implementation((TInput)input.Data)));
		    return this;
		}
		
		
		public FlowOperationContainer AddAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont) => implementation((TInput)input.Data, 
															                    output => outputCont(new Message(name, output)))
						   ));
		    return this;
		}
		

        public FlowOperationContainer MakeSync()
        {
            var asyncOp = _operations[_operations.Count - 1];
            _operations.RemoveAt(_operations.Count-1);

            var sync = new Synchronize<IMessage>();
            var syncOp = new Operation(asyncOp.Name, 
                                       (input, continueWith) => sync.Process(input, 
                                                                             output => asyncOp.Implementation(output, continueWith)));

            _operations.Add(syncOp);
            return this;
        }
		

		public IEnumerable<IOperation> Operations {	get { return _operations; }	}
	}
}

