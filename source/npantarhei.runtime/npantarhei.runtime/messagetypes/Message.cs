using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.messagetypes
{
	public class Message : IMessage
	{
		public Message(string portname) : this(portname, null) {}
		public Message(string portname, object data) : this(new Port(portname), data) {}
		public Message(IPort port, object data)
		{
			this.Port = port;
			this.Data = data;
			this.Causalities = new CausalityStack();
			this.FlowStack = new FlowStack();
		}

		#region IMessage implementation
		public IPort Port { get; private set; }
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
			return string.Format("Message(Port='{0}', Data='{1}', Causalities={2}, FlowStack depth={3})", this.Port, this.Data, !this.Causalities.IsEmpty, this.FlowStack.Depth);
		}
	}
}

