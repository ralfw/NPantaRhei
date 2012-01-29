using System;

namespace npantarhei.runtime.contract
{
	public interface IMessage {
		IPort Port {get;}
		object Data {get;}
	}
}

