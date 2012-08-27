using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace npantarhei.runtime.contract
{
    [Serializable]
    public class AsyncMethodAttribute : Attribute
    {
        private readonly string _threadPoolName;

        public AsyncMethodAttribute() : this("$$$asyncmethod$$$") {}
        public AsyncMethodAttribute(string threadPoolName)
        {
            _threadPoolName = threadPoolName;
        }

        public string ThreadPoolName { get { return _threadPoolName; } }
    }
}
