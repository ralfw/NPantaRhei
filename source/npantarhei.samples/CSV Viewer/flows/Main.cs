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
    class Main : IFlow
    {
        private readonly Formatter _formatter;
        private readonly Frontend _frontend;

        public Main(Formatter formatter, Frontend frontend)
        {
            _formatter = formatter;
            _frontend = frontend;
        }

        public IEnumerable<IStream> Streams
        {
            get { return FlowLoader.LoadFromEmbeddedResource("/", this.GetType().Assembly, "CSV_Viewer.flows.Main"); }
        }

        public IEnumerable<IOperation> Operations
        {
            get
            {
                return new FlowOperationContainer()
                    .AddFunc<Page, IEnumerable<string>>("format", _formatter.Format)
                    .AddAction<int>("menu", _frontend.Menu)
                    .AddAction<IEnumerable<string>>("output", _frontend.Output, true)
                    .Operations;
            }
        }
    }
}
