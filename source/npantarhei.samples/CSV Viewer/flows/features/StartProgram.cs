using System.Collections.Generic;
using CSV_Viewer.buffer;
using CSV_Viewer.environment;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace CSV_Viewer.flows.features
{
    class StartProgram : Flow
    {
        private readonly CommandlineParser _parser;
        private readonly TextFileAdapter _adapter;
        private readonly LineBuffer _buffer;

        public StartProgram(CommandlineParser parser, TextFileAdapter adapter, LineBuffer buffer) : base("start_program")
        {
            _parser = parser;
            _adapter = adapter;
            _buffer = buffer;
        }

        protected override IEnumerable<IStream> BuildStreams()
        {
            return new[]
                       {
                           new Stream(".in", "parse"),
                           new Stream("parse", "load_lines"),
                           new Stream("load_lines", "buffer_lines"),
                           new Stream("buffer_lines", ".out")
                       };
        }

        protected override IEnumerable<IOperation> BuildOperations(FlowOperationContainer container)
        {
            return container
                .AddFunc<string[], string>("parse", _parser.Parse)
                .AddFunc<string, IEnumerable<string>>("load_lines", _adapter.Read_all_lines)
                .AddAction<IEnumerable<string>>("buffer_lines", _buffer.Buffer_lines)
                .Operations;
        }
    }
}
