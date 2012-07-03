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
			var inputPorts = _streams.Where(s => s.FromPort.Fullname.ToLower() == outputMessage.Port.Fullname.ToLower())
									 .Select(s => s.ToPort)
									 .ToArray();

			// it is explicitly no error if no input port was found!
			// an output message can be generated without a corresponding input.
			// it´s like a ray of light hitting no eye.
			
			foreach(var port in inputPorts)
				Result(new Message(port, outputMessage.Data, outputMessage.CorrelationId){Causalities = outputMessage.Causalities, FlowStack = outputMessage.FlowStack});	
		}
		
		
		public event Action<IMessage> Result;
	}
}

