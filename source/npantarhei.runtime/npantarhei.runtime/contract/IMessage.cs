using System;
using System.Collections.Generic;

namespace npantarhei.runtime.contract
{
	public interface IMessage {
		IPort Port {get;}
        Guid CorrelationId { get; }
		object Data {get;}
	    CausalityStack Causalities {get; set;}
        FlowStack FlowStack {get; set;}
	}
}

