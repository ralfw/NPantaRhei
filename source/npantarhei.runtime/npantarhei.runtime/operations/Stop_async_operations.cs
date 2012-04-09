using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.operations
{
    public class Stop_async_operations
    {
        public void Process()
        {
            foreach(var op in _operations.Values)
                if (op as AsyncOperation != null) ((AsyncOperation)op).Stop();
        }


        private Dictionary<string, IOperation> _operations;
        public void Inject(Dictionary<string, IOperation> operations)
        {
            _operations = operations;
        }
    }
}