using System;

using npantarhei.runtime.contract;
using npantarhei.runtime.operations;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.flows
{
	public class Schedule_processing
	{
		public Schedule_processing ()
		{	
			var sel = new Select_processing_mode();
			var seq = new Serialize<IMessage>();
			var par = new Parallelize<IMessage>();
			
			_process = sel.Process;
			sel.ContinueSync += _ => Result(_);
		    sel.ContinueSequential += _ => Make_message_port_afine(_, seq.Enqueue);
			sel.ContinueParallel += par.Enqueue;
			seq.Dequeued += _ => Result(_);
			par.Dequeued += _ => Result(_);
			
            //TODO: No stop yet
			seq.Start();
			par.Start();
		}


        private void Make_message_port_afine(IMessage msg, Action<Tuple<IMessage, string>> continueWith)
        {
            continueWith(new Tuple<IMessage, string>(msg, msg.Port.Fullname));
        }
		
		private readonly Action<IMessage> _process;
		public void Process(IMessage message) { _process(message); }
		
		public event Action<IMessage> Result;
	}
}

