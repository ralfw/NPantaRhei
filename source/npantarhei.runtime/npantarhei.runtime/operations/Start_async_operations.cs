using System.Collections.Generic;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    public class Start_async_operations
    {
        public void Process()
        {
            foreach(var op in _operations.Values)
                if (op as AsyncWrapperOperation != null) ((AsyncWrapperOperation)op).Start();
        }


        private Dictionary<string, IOperation> _operations;
        public void Inject(Dictionary<string, IOperation> operations)
        {
            _operations = operations;
        }
    }
}