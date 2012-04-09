using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.contract
{
    interface IOperationImplementationWrapper<T>
    {
        void Process(T message, Action<T> continueWith);
    }
}
