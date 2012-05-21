using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;
using CSV_Viewer.pager;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace CSV_Viewer.flows.features
{
    class GetLastPage : Flow
    {
        private readonly Pager _pager;

        public GetLastPage(Pager pager) : base("get_last_page")
        {
            _pager = pager;
        }

        protected override IEnumerable<npantarhei.runtime.contract.IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".in", "goto_last_page"),
                           new Stream("goto_last_page", ".out")
                       };
        }

        protected override IEnumerable<npantarhei.runtime.contract.IOperation> BuildOperations(npantarhei.runtime.FlowOperationContainer container)
        {
            return container
                        .AddFunc<Page>("goto_last_page", _pager.LoadLast)
                        .Operations;
        }
    }
}
