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
            WrapLastOperation(sync);
            return this;
        }


        private readonly Dictionary<string, Asynchronize2<IMessage>> _asynchronizingOps = new Dictionary<string, Asynchronize2<IMessage>>();
        public FlowOperationContainer MakeAsync() { return MakeAsync("~~~async~~~"); }
        public FlowOperationContainer MakeAsync(string name)
        {
            Asynchronize2<IMessage> async;
            if (!_asynchronizingOps.TryGetValue(name, out async))
            {
                async = new Asynchronize2<IMessage>();
                async.Start();
                _asynchronizingOps.Add(name, async);
            }
            
            WrapLastOperation(async);
            
            return this;
        }


        private readonly Dictionary<string, Parallelize2<IMessage>> _parallelizingOps = new Dictionary<string, Parallelize2<IMessage>>();
        public FlowOperationContainer MakeParallel() { return MakeParallel("~~~parallel~~~"); }
        public FlowOperationContainer MakeParallel(string name)
        {
            Parallelize2<IMessage> parallel;
            if (!_parallelizingOps.TryGetValue(name, out parallel))
            {
                parallel = new Parallelize2<IMessage>();
                parallel.Start();
                _parallelizingOps.Add(name, parallel);
            }

            WrapLastOperation(parallel);

            return this;
        }


        private readonly Dictionary<string, Serialize2<IMessage>> _serializingOps = new Dictionary<string, Serialize2<IMessage>>();
        public FlowOperationContainer MakeSerial() { return MakeSerial("~~~serial~~~"); }
        public FlowOperationContainer MakeSerial(string name)
        {
            Serialize2<IMessage> serial;
            if (!_serializingOps.TryGetValue(name, out serial))
            {
                serial = new Serialize2<IMessage>(_ => _.Port.Fullname);
                serial.Start();
                _serializingOps.Add(name, serial);
            }

            WrapLastOperation(serial);

            return this;
        }


        private void WrapLastOperation(IOperationImplementationWrapper<IMessage> wrapper)
        {
            var op = _operations[_operations.Count - 1];
            _operations.RemoveAt(_operations.Count - 1);

            var wrappingOp = new Operation(op.Name,
                                          (input, continueWith) => wrapper.Process(input,
                                                                                   output => op.Implementation(output, continueWith)));
            _operations.Add(wrappingOp);
        }
		

		public IEnumerable<IOperation> Operations {	get { return _operations; }	}
	}
}

