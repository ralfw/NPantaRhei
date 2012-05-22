using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns.flows
{
    public abstract class LoadedFlow : AFlow
    {
        protected LoadedFlow(string name) : base(name) {}

        public override IEnumerable<IStream> Streams { get { return BuildStreams(); } }
    }
}
