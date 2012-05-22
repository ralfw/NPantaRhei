using System;
using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns.flows
{
    public class EmbeddedResourceFlow : AFlow
    {
        private readonly IEnumerable<IStream> _streams;

        public EmbeddedResourceFlow(string name, Type typeInResourceAssembly, string resourcename) : base(name)
        {
            _streams = FlowLoader.LoadFromEmbeddedResource(name, typeInResourceAssembly, resourcename);
        }

        protected override IEnumerable<IStream> BuildStreams() { return _streams; }
    }
}