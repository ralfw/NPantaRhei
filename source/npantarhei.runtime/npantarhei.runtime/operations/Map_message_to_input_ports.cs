using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
	internal class Map_message_to_input_ports
	{
		private List<IStream> _streams;
		
		public void Inject(List<IStream> streams)
		{
			_streams = streams;
		}
		

		public void Process(IMessage outputMessage)
		{
			var inputPorts = _streams.Where(s => s.FromPort.Fullname == outputMessage.Port.Fullname)
				    				 .Select(s => s.ToPort)
                                     .ToArray();
			
			if (!inputPorts.Any())
				throw new InvalidOperationException(string.Format("Unknown output port: '{0}'!", outputMessage.Port.Fullname));
			
			foreach(var port in inputPorts)
				Result(new Message(port, outputMessage.Data){Causalities = outputMessage.Causalities});	
		}
		
		
		public event Action<IMessage> Result;
	}
}

