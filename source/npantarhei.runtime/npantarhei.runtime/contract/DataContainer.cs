using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.contract
{
    public class DataContainer<T>
    {
        public DataContainer() {}
        public DataContainer(T data) { this.Data = data; }
 
        public void Initialize(T data)
        {
            this.Data = data;
            Initialized();
        }

        public T Data { get; private set; }

        public event Action Initialized = () => { };
    }
}
