using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Message : IMessage
	{
		public Message(string portname) : this(portname, null) {}
		public Message(string portname, Guid correlationId) : this(portname, null, correlationId) { }
		public Message(string portname, object data) : this(portname, data, Guid.Empty) {}
		public Message(string portname, object data, Guid correlationId) : this(new Port(portname), data, correlationId) { }
		public Message(IPort port, object data) : this(port, data, Guid.Empty) {}
		public Message(IPort port, object data, Guid correlationId)
		{
			this.Port = port;
			this.CorrelationId = correlationId;
			this.Data = data;
			this.Causalities = new CausalityStack();
			this.FlowStack = new FlowStack();
		}

		#region IMessage implementation
		public IPort Port { get; private set; }
		public Guid CorrelationId { get; set; }
		public object Data { get; private set; }

		private CausalityStack _causalities;
		public CausalityStack Causalities 
		{
			get { return _causalities; }
			set { _causalities = value.Copy(); }
		}

		private FlowStack _flowstack;
		public FlowStack FlowStack
		{
			get { return _flowstack; }
			set { _flowstack = value.Copy(); }
		}
		#endregion

		public override string ToString()
		{
			return string.Format("Message(Port='{0}', Data='{1}', Causalities={2}, FlowStack depth={3}, CorrelationId={4})", this.Port, this.Data, !this.Causalities.IsEmpty, this.FlowStack.Depth, Extract_string(this.CorrelationId.ToString(), 6));
		}

		private static string Extract_string(string text, int maxLength)
		{
			if (text.Length <= maxLength || text.Length < 3) return text;
			return text.Substring(0, 3) + ".." + text.Substring(text.Length - 3, 3);
		}
	}
}

