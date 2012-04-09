using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;
using npantarhei.runtime.operations;
using npantarhei.runtime.data;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.flows
{
	internal class Flow_asynchronously
	{
		private readonly Process_message _processMessage;
		
		public Flow_asynchronously()
		{
			// Build
			var async = new Asynchronize<IMessage>();
		    var handleException = new Handle_exception();
			_processMessage = new Process_message();

            Action<IMessage> enqueue = _ => async.Process(_, output => 
                                                                      {
                                                                          Message(_);
                                                                          handleException.Process(output);
                                                                      });

			// Bind
		    _process += enqueue;
            handleException.ContinueWith += _processMessage.Process;
		    handleException.ExceptionCaught += _ => UnhandledException(_);
		    _processMessage.Continue += enqueue;
            _processMessage.Result += _ => Message(_);
			_processMessage.Result += _ => Result(_);
			
			_start += async.Start;
			_stop += async.Stop;
		}
		
		public void Inject(List<IStream> streams, Dictionary<string, IOperation> operations)
		{
			_processMessage.Inject(streams, operations);
		}
		
		private readonly Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }

	    public event Action<IMessage> Message;
		public event Action<IMessage> Result;
	    public event Action<FlowRuntimeException> UnhandledException;
		
		private Action _start;
		public void Start() { _start(); }
		
		private Action _stop;
		public void Stop() { _stop(); }
	}
}

