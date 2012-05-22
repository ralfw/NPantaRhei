using System;
using System.Collections.Generic;
using System.Linq;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.flows
{
    public class Flow : AFlow, IFlow
    {
        public Flow(string name) : base(name) {}


        public override IEnumerable<IStream> Streams { get { return Qualify_streams(BuildStreams()); } }

        private IEnumerable<IStream> Qualify_streams(IEnumerable<IStream> streams)
        {
            return streams.Select(s => new Stream(Qualify_port(s.FromPort), Qualify_port(s.ToPort)));
        }

        private string Qualify_port(IPort port)
        {
            return port.IsQualified ? port.Fullname.Substring(1) : Build_qualified_port(port);
        }

        private string Build_qualified_port(IPort port)
        {
            return port.HasOperation
                       ? string.Format("{0}/{1}{2}", base.Name, port.OperationName, Build_portname(port))
                       : string.Format("{0}/{0}{1}", base.Name, Build_portname(port));
        }

        private string Build_portname(IPort port)
        {
            return port.Name == "" ? "" : "." + port.Name;
        }
    }
}
