using System;
using System.Collections.Generic;

using npantarhei.runtime.contract;

namespace npantarhei.runtime.operations
{
	internal class Register_stream
	{
		private List<IStream> _streams;
		
		public void Inject(List<IStream> streams)
		{
			_streams = streams;
		}
	
		
		public void Process(IStream stream)
		{
			_streams.Add(stream);
		}
	}
}

