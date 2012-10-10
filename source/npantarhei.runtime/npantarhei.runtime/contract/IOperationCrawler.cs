using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.contract
{
    public interface IOperationCrawler
    {
        void Register(Action<Type> registerStaticMethods, 
                      Action<object> registerInstanceMethods,
                      Action<object> registerEventBasedComponent);
    }
}
