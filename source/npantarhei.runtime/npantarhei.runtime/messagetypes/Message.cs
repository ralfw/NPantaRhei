using System;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Message : IMessage
	{
		public Message(object data) : this("", data) {}
		public Message(string portname, object data) : this(new Port(portname), data) {}
		public Message(IPort port, object data)
		{
			this.Port = port;
			this.Data = data;
		}

		#region IMessage implementation
		public IPort Port { get; private set; }
		public object Data { get; private set; }
		#endregion
	}
}

