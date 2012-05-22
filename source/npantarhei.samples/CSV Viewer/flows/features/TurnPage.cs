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
    class TurnPage : Flow
    {
        private readonly Pager _pager;

        public TurnPage(Pager pager) : base("turn_page")
        {
            _pager = pager;
        }

        protected override IEnumerable<npantarhei.runtime.contract.IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream("/get_next_page/get_next_page.in", "/get_next_page/goto_next_page"),
                           new Stream("/get_next_page/goto_next_page", "/get_next_page/get_next_page.out"),

                           new Stream("/get_prev_page/get_prev_page.in", "/get_prev_page/goto_prev_page"),
                           new Stream("/get_prev_page/goto_prev_page", "/get_prev_page/get_prev_page.out"), 
                       };
        }

        protected override IEnumerable<npantarhei.runtime.contract.IOperation> BuildOperations(npantarhei.runtime.FlowOperationContainer container)
        {
            return container
                        .AddFunc<Page>("goto_next_page", _pager.LoadNext)
                        .AddFunc<Page>("goto_prev_page", _pager.LoadPrev)
                        .Operations;
        }
    }
}
