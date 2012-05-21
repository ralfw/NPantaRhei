using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;
using npantarhei.runtime.contract;

namespace CSV_Viewer.buffer
{
    class LineBuffer
    {
        private readonly DataContainer<PageBuffer> _container;

        public LineBuffer(DataContainer<PageBuffer> container)
        {
            _container = container;
        }

        public void Buffer_lines(IEnumerable<string> lines, Action enough_data_buffered_to_continue)
        {
            foreach(var l in lines) _container.Data.AddLine(l);
            enough_data_buffered_to_continue();
        }
    }
}
