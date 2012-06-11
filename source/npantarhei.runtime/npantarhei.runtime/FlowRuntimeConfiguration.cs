using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;
using System.Reflection;
using System.IO;

namespace npantarhei.runtime
{
	public class FlowRuntimeConfiguration
	{
		public static Func<ISynchronizationBuilder> SynchronizationBuilderFactory { get; set; }

		static FlowRuntimeConfiguration()
		{
			SynchronizationBuilderFactory = () => new SynchronizationContextBuilder();
		}

		#region Operations
		private readonly List<IOperation> _operations = new List<IOperation>();

		public IEnumerable<IOperation> Operations { get { return _operations; } }


		public FlowRuntimeConfiguration AddOperation(IOperation operation) { _operations.Add(operation); return this; }
		public FlowRuntimeConfiguration AddOperations(IEnumerable<IOperation> operations) { _operations.AddRange(operations); return this; }


		public FlowRuntimeConfiguration AddFunc<TOutput>(string name, Func<TOutput> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => { var result = implementation();
																	  outputCont(new Message(name, result)); }
						   ));
			return this;
		}

		public FlowRuntimeConfiguration AddFunc<TInput, TOutput>(string name, Func<TInput, TOutput> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont, _) => { var result = implementation((TInput)input.Data);
																	  outputCont(new Message(name, result)); }
						   ));
			return this;
		}


		public FlowRuntimeConfiguration AddAction(string name, Action implementation, bool createContinuationSignal = false)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => { implementation(); 
																			if (createContinuationSignal) outputCont(new Message(name, null)); }));
			return this;
		}
		
		public FlowRuntimeConfiguration AddAction<TInput>(string name, Action<TInput> implementation, bool createContinuationSignal = false)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => { implementation((TInput)input.Data); 
																			if (createContinuationSignal) outputCont(new Message(name, null)); }));
			return this;
		}
		
		public FlowRuntimeConfiguration AddAction<TInput>(string name, Action<TInput, Action> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation((TInput)input.Data,
																				   () => outputCont(new Message(name, null)))
						   ));
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => implementation((TInput)input.Data, 
																						 output => outputCont(new Message(name, output)))));
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TOutput>(string name, Action<Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => implementation(output => outputCont(new Message(name, output)))));
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput, TOutput0, TOutput1>(string name, Action<TInput, Action<TOutput0>, Action<TOutput1>> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation((TInput)input.Data,
																				   output0 => outputCont(new Message(name + ".out0", output0)),
																				   output1 => outputCont(new Message(name + ".out1", output1)))
						   ));
			return this;
		}


		public FlowRuntimeConfiguration AddAutoResetJoin<T0, T1>(string name)
		{
			_operations.Add(new AutoResetJoin<T0, T1>(name));
			return this;
		}

		public FlowRuntimeConfiguration AddAutoResetJoin<T0, T1, T2>(string name)
		{
			_operations.Add(new AutoResetJoin<T0, T1, T2>(name));
			return this;
		}

		public FlowRuntimeConfiguration AddManualResetJoin<T0, T1>(string name)
		{
			_operations.Add(new ManualResetJoin<T0, T1>(name));
			return this;
		}

		public FlowRuntimeConfiguration AddManualResetJoin<T0, T1, T2>(string name)
		{
			_operations.Add(new ManualResetJoin<T0, T1, T2>(name));
			return this;
		}


		public FlowRuntimeConfiguration AddPushCausality(string name)
		{
			_operations.Add(new PushCausality(name));
			return this;
		}

		public FlowRuntimeConfiguration AddPopCausality(string name)
		{
			_operations.Add(new PopCausality(name));
			return this;
		}


		public FlowRuntimeConfiguration MakeSync()
		{
			var sync = FlowRuntimeConfiguration.SynchronizationBuilderFactory();
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
		public FlowRuntimeConfiguration MakeAsync() { return MakeAsync("~~~async~~~"); }
		public FlowRuntimeConfiguration MakeAsync(string name)
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
		public FlowRuntimeConfiguration MakeParallel() { return MakeParallel("~~~parallel~~~"); }
		public FlowRuntimeConfiguration MakeParallel(string name)
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
		public FlowRuntimeConfiguration MakeSerial() { return MakeSerial("~~~serial~~~"); }
		public FlowRuntimeConfiguration MakeSerial(string name)
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
		#endregion

		#region Streams
		private readonly List<IStream> _streams = new List<IStream>();

		public IEnumerable<IStream> Streams { get { return _streams; } }


		public FlowRuntimeConfiguration AddStream(string fromPortName, string toPortName) { AddStream(new messagetypes.Stream(fromPortName, toPortName)); return this; }
		public FlowRuntimeConfiguration AddStream(IStream stream) { _streams.Add(stream); return this; }
		public FlowRuntimeConfiguration AddStreams(IEnumerable<IStream> streams) { streams.ToList().ForEach(_ => this.AddStream(_)); return this; }

		public FlowRuntimeConfiguration AddStreamsFrom(string resourceName, Assembly resourceAssembly) { this.AddStreams(FlowLoader.LoadFromEmbeddedResource(FlowLoader.ROOT_FLOW_NAME, resourceAssembly, resourceName)); return this; }
		public FlowRuntimeConfiguration AddStreamsFrom(IEnumerable<string> lines) { this.AddStreams(FlowLoader.LoadFromLines(FlowLoader.ROOT_FLOW_NAME, lines)); return this; }
		public FlowRuntimeConfiguration AddStreamsFrom(string text) { this.AddStreams(FlowLoader.LoadFromReader(FlowLoader.ROOT_FLOW_NAME, new StringReader(text))); return this; }
		#endregion

		public void AddFlow(IFlow flow)
		{
			this.AddStreams(flow.Streams);
			this.AddOperations(flow.Operations);
		}
	}
}

