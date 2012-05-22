using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;
using CSV_Viewer.pager;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.flows;
using npantarhei.runtime.patterns.operations;

namespace CSV_Viewer.flows.features
{
    class GetFirstPage : Flow
    {
        private readonly Pager _pager;

        public GetFirstPage(Pager pager) : base("get_first_page")
        {
            _pager = pager;
        }

        protected override IEnumerable<npantarhei.runtime.contract.IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".in", "goto_first_page"),
                           new Stream("goto_first_page", ".out")
                       };
        }

        protected override IEnumerable<npantarhei.runtime.contract.IOperation> BuildOperations(npantarhei.runtime.FlowOperationContainer container)
        {
            return container
                        .AddFunc<Page>("goto_first_page", _pager.LoadFirst)
                        .Operations;
        }
    }
}
