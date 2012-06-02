using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.buffer;
using CSV_Viewer.data_model;
using CSV_Viewer.pager;
using CSV_Viewer.resources;
using npantarhei.runtime;
using npantarhei.runtime.contract;

namespace CSV_Viewer.flows
{
    class Features : IFlow
    {
        private readonly CommandlineParser _parser;
        private readonly TextFileAdapter _adapter;
        private readonly LineBuffer _buffer;
        private readonly Pager _pager;

        public Features(CommandlineParser parser, TextFileAdapter adapter, LineBuffer buffer, Pager pager)
        {
            _parser = parser;
            _adapter = adapter;
            _buffer = buffer;
            _pager = pager;
        }

        public IEnumerable<IStream> Streams
        {
            get { return FlowLoader.LoadFromEmbeddedResource("/", this.GetType().Assembly, "CSV_Viewer.flows.Features.flow"); }
        }

        public IEnumerable<IOperation> Operations
        {
            get
            {
                return new FlowOperationContainer()
                                .AddAction<IEnumerable<string>>("buffer_lines", _buffer.Buffer_lines)
                                .AddFunc<Page>("goto_first_page", _pager.LoadFirst)
                                .AddFunc<Page>("goto_last_page", _pager.LoadLast)
                                .AddFunc<Page>("goto_next_page", _pager.LoadNext)
                                .AddFunc<Page>("get_prev_page", _pager.LoadPrev) // function replacing former flow
                                .AddFunc<string, IEnumerable<string>>("load_lines", _adapter.Read_all_lines)
                                .AddFunc<string[], string>("parse", _parser.Parse)
                                .Operations;
            }
        }
    }
}
