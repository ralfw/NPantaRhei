using System;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	public class Select_processing_mode
	{		
		public void Process(IMessage message)
		{
			switch(message.Port.ProcessingMode)
			{
				case PortProcessingModes.Sequential: ContinueSequential(message); break;
				case PortProcessingModes.Parallel: ContinueParallel(message); break;
				default: ContinueSync(message); break;
			}
		}
		
		public event Action<IMessage> ContinueSync;
		public event Action<IMessage> ContinueSequential;
		public event Action<IMessage> ContinueParallel;
	}
}

