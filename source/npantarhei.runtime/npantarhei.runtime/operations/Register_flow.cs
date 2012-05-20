using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.operations
{
	internal class Register_flow
	{
		public void Process(IOperation operation)
		{
			var flow = operation as IFlow;
			if (flow == null) return;

            flow.Streams.ToList().ForEach(RegisterStream);
            flow.Operations.ToList().ForEach(RegisterOperation);
		}


		public Action<IStream> RegisterStream;
		public Action<IOperation> RegisterOperation;
	}
}

