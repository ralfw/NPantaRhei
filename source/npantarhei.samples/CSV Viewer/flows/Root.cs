using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.flows;

namespace CSV_Viewer.flows
{
    class Root : Flow
    {
        private readonly Main _main;

        public Root(Main main) : base("*") { _main = main; }

        protected override IEnumerable<npantarhei.runtime.contract.IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".run", "main.run"),
                           new Stream("main.exit", ".exit"),
                           new Stream(".displayFirstPage", "main.displayFirstPage"),
                           new Stream(".displayLastPage", "main.displayLastPage"),
                           new Stream(".displayNextPage", "main.displayNextPage"),
                           new Stream(".displayPrevPage", "main.displayPrevPage")
                       };
        }

        protected override IEnumerable<npantarhei.runtime.contract.IOperation> BuildOperations(npantarhei.runtime.FlowOperationContainer container)
        {
            return new[] {_main};
        }
    }
}
