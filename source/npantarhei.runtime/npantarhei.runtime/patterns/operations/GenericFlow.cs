using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    public class GenericFlow : Flow
    {
        public GenericFlow(string name) : base(name) {}

        protected override IEnumerable<IStream> BuildStreams() { return new Stream[] { }; }
    }
}
