using System.Collections.Generic;
using System.IO;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns.flows
{
    public class StringFlow : AFlow
    {
        private readonly IEnumerable<IStream> _streams;

        public StringFlow(string name, string source) : base(name)
        {
            _streams = FlowLoader.LoadFromReader(name, new StringReader(source));
        }

        protected override IEnumerable<IStream> BuildStreams() { return _streams; }
    }
}
