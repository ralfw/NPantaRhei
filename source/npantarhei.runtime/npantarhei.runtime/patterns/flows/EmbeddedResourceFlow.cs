using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns.flows
{
    public class EmbeddedResourceFlow : Flow
    {
        private readonly IEnumerable<IStream> _streams;

        public EmbeddedResourceFlow(string name, string resourcename) : base(name)
        {
            _streams = FlowLoader.LoadFromEmbeddedResource(name, this.GetType(), resourcename);
        }

        protected override IEnumerable<IStream> BuildStreams() { return _streams; }
    }
}