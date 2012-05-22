using System.Collections.Generic;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns.flows
{
    public class FileFlow : Flow
    {
        private readonly IEnumerable<IStream> _streams; 

        public FileFlow(string name, string filename) : base(name)
        {
            _streams = FlowLoader.LoadFromFile(name, filename);
        }

        protected override IEnumerable<IStream> BuildStreams() { return _streams; }
    }
}
