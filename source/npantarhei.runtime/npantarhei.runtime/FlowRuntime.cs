using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.flows;
using Stream = npantarhei.runtime.messagetypes.Stream;

namespace npantarhei.runtime
{
	public class FlowRuntime : IFlowRuntime
	{		
		public FlowRuntime() : this(new FlowRuntimeConfiguration()) {}
		public FlowRuntime(FlowRuntimeConfiguration config) : this(config, new Schedule_for_async_depthfirst_processing()) {}
		public FlowRuntime(FlowRuntimeConfiguration config, IScheduler schedule)
		{
			// Build
			var regStream = new Register_stream();
			var regOp = new Register_operation();
			var configOp = new Create_activation_task();
			
			var flow = new Flow_asynchronously(schedule);

			var opStart = new Start_async_operations();
			var opStop = new Stop_async_operations();
			
			// Bind
			_addStream += regStream.Process;
			_addOperation += regOp.Process;
			_addOperation += configOp.Process;
			configOp.Result += flow.Execute;
			
			_process += flow.Process;
			flow.Message += _ => Message(_);
			flow.Result += Pass_result_to_environment;
			flow.UnhandledException += ex =>
										   {
											   if (UnhandledException == null) 
												   throw new FlowRuntimeException("Unhandled exception! Missing causality or global exception handler.", ex.InnerException, ex.Context);
											   UnhandledException(ex);
										   };

			_start += opStart.Process;
			_start += flow.Start;

			_stop += flow.Stop;
			_stop += opStop.Process;

			_throttle += flow.Throttle;
			
			// Inject
			var streams = new List<IStream>();
			regStream.Inject(streams);
			
			var operations = new Dictionary<string, IOperation>();
			regOp.Inject(operations);
			
			flow.Inject(streams, operations);
			opStart.Inject(operations);
			opStop.Inject(operations);

			// Config
			Configure(config);

			// Run
			Start();
		}


		private readonly ConcurrentQueue<IMessage> _resultBuffer = new ConcurrentQueue<IMessage>();
		private void Pass_result_to_environment(IMessage message)
		{
			if (_result == null)
				Buffer_result(message);
			else
				_result(message);
		}

		private void Buffer_result(IMessage message)
		{
			_resultBuffer.Enqueue(message);
		}

		private void Empty_result_buffer()
		{
			IMessage previousMessage = null;
			while (_resultBuffer.TryDequeue(out previousMessage))
				_result(previousMessage);
		}


		#region IFlowRuntime implementation
		public void Configure(FlowRuntimeConfiguration config)
		{
			this.AddStreams(config.Streams);
			this.AddOperations(config.Operations);
		}

		private readonly Action<IMessage> _process;
		public void Process(string portname) { Process(portname, null); }
		public void Process(string portname, object data) { Process(new Message(portname, data)); }
		public void Process(IMessage message) { _process(message); }

		public event Action<IMessage> Message = _ => { };
		public event Action<FlowRuntimeException> UnhandledException;

		private event Action<IMessage> _result;
		public event Action<IMessage> Result { 
			add
			{
				_result += value;
				Empty_result_buffer();
			}
			remove { _result -= value; }
		}

		public bool WaitForResult() { return WaitForResult(Timeout.Infinite); }
		public bool WaitForResult(int milliseconds) { return WaitForResult(milliseconds, _ => {}); }
		public bool WaitForResult(Action<IMessage> processResult) { return WaitForResult(Timeout.Infinite, processResult); }
		public bool WaitForResult(int milliseconds, Action<IMessage> processResult)
		{
			var are = new AutoResetEvent(false);
			this.Result += _ =>
							   {
								   processResult(_);
								   are.Set();
							   };
			return are.WaitOne(milliseconds);
		}
		
		public Action CreateEventProcessor(string portname) { return () => this.Process(new Message(portname, null)); }
		public Action<T> CreateEventProcessor<T>(string portname) { return data => this.Process(new Message(portname, data)); }

		private readonly Action<int> _throttle;
		public void Throttle(int delayMilliseconds) { _throttle(delayMilliseconds); }
		#endregion


		private readonly Action _start;
		private void Start() { _start(); }

		private readonly Action _stop;
		private void Stop() { _stop(); }
		
		public void Dispose() { Stop(); }


		private readonly Action<IStream> _addStream;
		private void AddStreams(IEnumerable<IStream> streams) { streams.ToList().ForEach(_addStream); }

		private readonly Action<IOperation> _addOperation;
		private void AddOperations(IEnumerable<IOperation> operations) { operations.ToList().ForEach(_addOperation); }
	}
}

