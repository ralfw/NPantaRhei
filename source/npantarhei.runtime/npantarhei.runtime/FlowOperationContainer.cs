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
            var sync = new Synchronize<IMessage>();
            WrapLastOperation(op => new Operation(op.Name, (input, continueWith) => sync.Process(input,
                                                                                                 output => op.Implementation(output, continueWith))));
            return this;
        }


        private readonly Dictionary<string, Asynchronize<IMessage>> _asynchronizingOps = new Dictionary<string, Asynchronize<IMessage>>();
        public FlowOperationContainer MakeAsync() { return MakeAsync("~~~async~~~"); }
        public FlowOperationContainer MakeAsync(string name)
        {
            Asynchronize<IMessage> async;
            if (!_asynchronizingOps.TryGetValue(name, out async))
            {
                async = new Asynchronize<IMessage>();
                async.Start();
                _asynchronizingOps.Add(name, async);
            }

            WrapLastOperation(op => new AsyncWrapperOperation(async, op));
            
            return this;
        }


        private readonly Dictionary<string, Parallelize<IMessage>> _parallelizingOps = new Dictionary<string, Parallelize<IMessage>>();
        public FlowOperationContainer MakeParallel() { return MakeParallel("~~~parallel~~~"); }
        public FlowOperationContainer MakeParallel(string name)
        {
            Parallelize<IMessage> parallel;
            if (!_parallelizingOps.TryGetValue(name, out parallel))
            {
                parallel = new Parallelize<IMessage>();
                parallel.Start();
                _parallelizingOps.Add(name, parallel);
            }

            WrapLastOperation(op => new AsyncWrapperOperation(parallel, op));

            return this;
        }


        private readonly Dictionary<string, Serialize<IMessage>> _serializingOps = new Dictionary<string, Serialize<IMessage>>();
        public FlowOperationContainer MakeSerial() { return MakeSerial("~~~serial~~~"); }
        public FlowOperationContainer MakeSerial(string name)
        {
            Serialize<IMessage> serial;
            if (!_serializingOps.TryGetValue(name, out serial))
            {
                serial = new Serialize<IMessage>(_ => _.Port.Fullname);
                serial.Start();
                _serializingOps.Add(name, serial);
            };

            WrapLastOperation(op => new AsyncWrapperOperation(serial, op));

            return this;
        }


        private void WrapLastOperation(Func<IOperation, IOperation> wrappingOperationFactory)
        {
            var op = _operations[_operations.Count - 1];
            _operations.RemoveAt(_operations.Count - 1);

            var wrappingOp = wrappingOperationFactory(op);

            _operations.Add(wrappingOp);
        }
		

		public IEnumerable<IOperation> Operations {	get { return _operations; }	}
	}
}

