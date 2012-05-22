using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;
using CSV_Viewer.formatter;
using CSV_Viewer.portals;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace CSV_Viewer.flows
{
    class Main : Flow
    {
        private readonly IFlow _startProgram;
        private readonly IFlow _getFirstPage;
        private readonly IFlow _getLastPage;
        private readonly IFlow _turnPage;
        private readonly Formatter _formatter;
        private readonly Frontend _frontend;

        public Main(IFlow start_program, IFlow get_first_page, IFlow get_last_page, IFlow turn_page,
                    Formatter formatter, Frontend frontend) : base("main")
        {
            _startProgram = start_program;
            _getFirstPage = get_first_page;
            _getLastPage = get_last_page;
            _turnPage = turn_page;
            _formatter = formatter;
            _frontend = frontend;
        }

        protected override IEnumerable<IStream> BuildStreams()
        {
            return new[]
                       {
                           // main
                           new Stream(".run", "start_program.in"),
                           new Stream("start_program.out", "get_first_page.in"),
                           new Stream("display.out", "menu"),
                           new Stream("menu", ".exit"), 
 
                           new Stream(".displayFirstPage", "get_first_page.in"),
                           new Stream("get_first_page.out", "display.in"), 

                           new Stream(".displayLastPage", "get_last_page.in"),
                           new Stream("get_last_page.out", "display.in"),

                           new Stream(".displayNextPage", "get_next_page.in"),
                           new Stream("get_next_page.out", "display.in"),
 
                           new Stream(".displayPrevPage", "get_prev_page.in"),
                           new Stream("get_prev_page.out", "display.in"), 

                           // display
                           new Stream("/display/display.in", "/display/format"),
                           new Stream("/display/format", "/display/output"),
                           new Stream("/display/output", "/display/display.out") 
                       };
        }

        protected override IEnumerable<IOperation> BuildOperations(FlowOperationContainer container)
        {
            return container
                .AddFunc<Page,IEnumerable<string>>("format", _formatter.Format)
                .AddAction<int>("menu", _frontend.Menu)
                .AddAction<IEnumerable<string>>("output", _frontend.Output)
                .Operations
                .Concat(new[] {_startProgram, _getFirstPage, _getLastPage, _turnPage});
        }
    }
}
