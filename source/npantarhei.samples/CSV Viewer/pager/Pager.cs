using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSV_Viewer.data_model;
using npantarhei.runtime.contract;

namespace CSV_Viewer.pager
{
    class Pager
    {
        private readonly DataContainer<PageBuffer> _container;

        public Pager(DataContainer<PageBuffer> container)
        {
            _container = container;
        }

        public Page LoadFirst()
        {
            _container.Data.GotoFirst();
            return _container.Data.Current;
        }

        public Page LoadLast()
        {
            _container.Data.GotoLast();
            return _container.Data.Current;
        }

        public Page LoadNext()
        {
            _container.Data.GotoNext();
            return _container.Data.Current;
        }

        public Page LoadPrev()
        {
            _container.Data.GotoPrev();
            return _container.Data.Current;
        }
    }
}
