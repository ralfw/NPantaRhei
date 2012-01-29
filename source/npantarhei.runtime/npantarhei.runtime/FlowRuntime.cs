using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;
using npantarhei.runtime.flows;
using npantarhei.runtime.data;

namespace npantarhei.runtime
{
	public class FlowRuntime : IFlowRuntime
	{		
		public FlowRuntime()
		{
			// Build
			var regStream = new Register_stream();
			var regOp = new Register_operation();
			
			var flowSync = new Flow_synchronously();
			var flowAsync = new Flow_asynchronously();
			
			// Bind
			_addStream += regStream.Process;
			_addOperation += regOp.Process;
			
			_processSync += flowSync.Process;
			flowSync.Result += _ => Result(_);
			
			_processAsync += flowAsync.Process;
			flowAsync.Result += _ => Result(_);
			
			_start += flowAsync.Start;
			_stop += flowAsync.Stop;
			
			// Inject
			var streams = new List<IStream>();
			regStream.Inject(streams);
			
			var operations = new Dictionary<string, IOperation>();
			regOp.Inject(operations);
			
			flowSync.Inject(streams, operations);
			flowAsync.Inject(streams, operations);
		}
		
		#region IFlowRuntime implementation
		private Action<IMessage> _processSync;
		public void ProcessSync(IMessage message) { _processSync(message); }

		private Action<IMessage> _processAsync;
		public void ProcessAsync(IMessage message) { _processAsync(message); }
		
		public event Action<IMessage> Result;
		
		
		private Action<IStream> _addStream;
		public void AddStream (IStream stream) { _addStream(stream);}
	
		private Action<IOperation> _addOperation;
		public void AddOperation (IOperation operation) { _addOperation(operation); }
		public void AddOperations(IEnumerable<IOperation> operations) {
			operations.ToList().ForEach(op => this.AddOperation(op));
		}
		
		private Action _start;
		public void Start() { _start(); }
		
		private Action _stop;
		public void Stop() { _stop(); }
		#endregion
		
		
		public void Dispose() { Stop(); }
	}
}

