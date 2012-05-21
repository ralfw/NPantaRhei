using CSV_Viewer.data_model;
using npantarhei.runtime.contract;

namespace CSV_Viewer.resources
{
    class CommandlineParser
    {
        private readonly DataContainer<PageBuffer> _container;

        public CommandlineParser(DataContainer<PageBuffer> container)
        {
            _container = container;
        }

        public string Parse(string[] args)
        {
            _container.Initialize(new PageBuffer(3));
            return args[0];
        }
    }
}
