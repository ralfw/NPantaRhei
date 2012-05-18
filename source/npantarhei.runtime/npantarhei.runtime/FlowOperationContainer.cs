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
	    private readonly Func<ISynchronize<IMessage>> _synchronizeBuilder;

	    public FlowOperationContainer(Func<ISynchronize<IMessage>> synchronizeBuilder) {
	        _synchronizeBuilder = synchronizeBuilder;
	    }

	    public FlowOperationContainer()
            : this(() => new Synchronize<IMessage>()) {
	    }

	    public FlowOperationContainer AddFunc<TOutput>(string name, Func<TOutput> implementation)
        {
            _operations.Add(new Operation(name,
                                          (input, outputCont, _) => {
                                                var result = implementation();
                                                outputCont(new Message(name, result));
                                          }
                           ));
            return this;
        }

		public FlowOperationContainer AddFunc<TInput, TOutput>(string name, Func<TInput, TOutput> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont, _) => {
										  		var result = implementation((TInput)input.Data);
												outputCont(new Message(name, result));
										  }
						   ));
		    return this;
		}


        public FlowOperationContainer AddAction(string name, Action implementation)
        {
            _operations.Add(new Operation(name, (input, _, __) => implementation()));
            return this;
        }
		
		public FlowOperationContainer AddAction<TInput>(string name, Action<TInput> implementation)
		{
			_operations.Add(new Operation(name, (input, _, __) => implementation((TInput)input.Data)));
		    return this;
		}
		
        public FlowOperationContainer AddAction<TInput>(string name, Action<TInput, Action> implementation)
        {
            _operations.Add(new Operation(name,
                                          (input, outputCont, _) => implementation((TInput)input.Data,
                                                                                () => outputCont(new Message(name, null)))
                           ));
            return this;
        }

        public FlowOperationContainer AddAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont, _) => implementation((TInput)input.Data, 
															                    output => outputCont(new Message(name, output)))
						   ));
		    return this;
		}

        public FlowOperationContainer AddAction<TInput, TOutput0, TOutput1>(string name, Action<TInput, Action<TOutput0>, Action<TOutput1>> implementation)
        {
            _operations.Add(new Operation(name,
                                          (input, outputCont, _) => implementation((TInput)input.Data,
                                                                                   output0 => outputCont(new Message(name + ".out0", output0)),
                                                                                   output1 => outputCont(new Message(name + ".out1", output1)))
                           ));
            return this;
        }


        public FlowOperationContainer AddAutoResetJoin<T0, T1>(string name)
        {
            _operations.Add(new AutoResetJoin<T0, T1>(name));
            return this;
        }

        public FlowOperationContainer AddAutoResetJoin<T0, T1, T2>(string name)
        {
            _operations.Add(new AutoResetJoin<T0, T1, T2>(name));
            return this;
        }

        public FlowOperationContainer AddManualResetJoin<T0, T1>(string name)
        {
            _operations.Add(new ManualResetJoin<T0, T1>(name));
            return this;
        }

        public FlowOperationContainer AddManualResetJoin<T0, T1, T2>(string name)
        {
            _operations.Add(new ManualResetJoin<T0, T1, T2>(name));
            return this;
        }


        public FlowOperationContainer AddPushCausality(string name)
        {
            _operations.Add(new PushCausality(name));
            return this;
        }

        public FlowOperationContainer AddPopCausality(string name)
        {
            _operations.Add(new PopCausality(name));
            return this;
        }


        public FlowOperationContainer MakeSync()
        {
            var sync = _synchronizeBuilder();
            WrapLastOperation(op => new Operation(op.Name, (input, continueWith, unhandledException) => 
                                                                sync.Process(input,
                                                                             output =>
                                                                                 {
                                                                                     try
                                                                                     {
                                                                                         op.Implementation(output, continueWith, unhandledException);
                                                                                     }
                                                                                     catch (Exception ex)
                                                                                     {
                                                                                         unhandledException(new FlowRuntimeException(ex, output));
                                                                                     }
                                                                                 })));
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

