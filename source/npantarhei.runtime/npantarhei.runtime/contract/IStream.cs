using System;

namespace npantarhei.runtime.contract
{
	public interface IStream {
		IPort FromPort {get;}
		IPort ToPort {get;}
	}
}

