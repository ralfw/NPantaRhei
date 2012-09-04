using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;
using System.Reflection;
using System.IO;

namespace npantarhei.runtime
{
	public class FlowRuntimeConfiguration
	{        
		public static Func<IDispatcher> DispatcherFactory { get; set; }
		[Obsolete]
		public static Func<IDispatcher> SynchronizationFactory { get { return DispatcherFactory; } set { DispatcherFactory = value; } } 


		static FlowRuntimeConfiguration()
		{
			DispatcherFactory = () => new DispatchWithSynchronizationContext();
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
																	  outputCont(new Message(name, result, input.CorrelationId)); }
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddFunc<TInput, TOutput>(string name, Func<TInput, TOutput> implementation)
		{
			_operations.Add(new Operation(name, 
										  (input, outputCont, _) => { var result = implementation((TInput)input.Data);
																	  outputCont(new Message(name, result, input.CorrelationId)); }
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}


		public FlowRuntimeConfiguration AddAction(string name, Action implementation, bool createContinuationSignal = false)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => { implementation(); 
																			if (createContinuationSignal) outputCont(new Message(name, null, input.CorrelationId)); }));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}
		
		public FlowRuntimeConfiguration AddAction<TInput>(string name, Action<TInput> implementation, bool createContinuationSignal = false)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => { implementation((TInput)input.Data); 
																			if (createContinuationSignal) outputCont(new Message(name, null, input.CorrelationId)); }));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput>(string name, Action<TInput, Action> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation((TInput)input.Data,
																				   () => outputCont(new Message(name, null, input.CorrelationId)))
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => implementation((TInput)input.Data, 
																						 output => outputCont(new Message(name, output, input.CorrelationId)))));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TOutput>(string name, Action<Action<TOutput>> implementation)
		{
			_operations.Add(new Operation(name, (input, outputCont, _) => implementation(output => outputCont(new Message(name, output, input.CorrelationId)))));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction(string name, Action<Action, Action> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation(() => outputCont(new Message(name + ".out0", null, input.CorrelationId)),
																				   () => outputCont(new Message(name + ".out1", null, input.CorrelationId)))
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TOutput>(string name, Action<Action<TOutput>, Action> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation(output => outputCont(new Message(name + ".out0", output, input.CorrelationId)),
																				   () => outputCont(new Message(name + ".out1", null, input.CorrelationId)))
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput, TOutput>(string name, Action<TInput, Action<TOutput>, Action> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation((TInput)input.Data,
																				   output => outputCont(new Message(name + ".out0", output, input.CorrelationId)),
																				   () => outputCont(new Message(name + ".out1", null, input.CorrelationId)))
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}

		public FlowRuntimeConfiguration AddAction<TInput, TOutput0, TOutput1>(string name, Action<TInput, Action<TOutput0>, Action<TOutput1>> implementation)
		{
			_operations.Add(new Operation(name,
										  (input, outputCont, _) => implementation((TInput)input.Data,
																				   output0 => outputCont(new Message(name + ".out0", output0, input.CorrelationId)),
																				   output1 => outputCont(new Message(name + ".out1", output1, input.CorrelationId)))
						   ));
			Auto_apply_MakeDispatched(implementation);
			return this;
		}


		public FlowRuntimeConfiguration AddEventBasedComponent(string name, object eventBasedComponent)
		{
			_operations.Add(new EBCOperation(name, eventBasedComponent, FlowRuntimeConfiguration.DispatcherFactory(), _asyncerCache));
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


		public FlowRuntimeConfiguration MakeDispatched()
		{
			var sync = FlowRuntimeConfiguration.DispatcherFactory();
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
		[Obsolete]
		public FlowRuntimeConfiguration MakeSync() { return MakeDispatched(); }

		private void Auto_apply_MakeDispatched(Delegate implementation)
		{
			if (DispatchedMethodAttribute.HasBeenApplied(implementation)) MakeDispatched();
		}


		private readonly AsynchronizerCache _asyncerCache = new AsynchronizerCache();
		public FlowRuntimeConfiguration MakeAsync() { return MakeAsync("~~~async~~~"); }
		public FlowRuntimeConfiguration MakeAsync(string name)
		{
			WrapLastOperation(op => new AsyncWrapperOperation(_asyncerCache.Get(name, () => new AsynchronizeFIFO()), op));
			return this;
		}


		public FlowRuntimeConfiguration MakeParallel() { return MakeParallel("~~~parallel~~~"); }
		public FlowRuntimeConfiguration MakeParallel(string name)
		{
			WrapLastOperation(op => new AsyncWrapperOperation(_asyncerCache.Get(name, () => new Parallelize()), op));
			return this;
		}


		[Obsolete("Use MakeAsync() instead.")]
		public FlowRuntimeConfiguration MakeSerial() { return MakeSerial("~~~serial~~~"); }
		[Obsolete("Use MakeAsync() instead.")]
		public FlowRuntimeConfiguration MakeSerial(string name)
		{
			WrapLastOperation(op => new AsyncWrapperOperation(_asyncerCache.Get(name, () => new Serialize(_ => _.Port.Fullname)), op));
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

