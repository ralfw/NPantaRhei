using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Stream : IStream
	{
		public Stream(string fromPortname, string toPortname) : this(new Port(fromPortname), new Port(toPortname)) {}
		public Stream(IPort fromPort, IPort toPort)
		{
			this.FromPort = fromPort;
			this.ToPort = toPort;
		}
		
		#region IStream implementation
		public IPort FromPort { get; private set; }
		public IPort ToPort  { get; private set; }
		#endregion
	}
}

