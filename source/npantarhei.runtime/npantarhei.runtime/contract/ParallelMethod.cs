using System;

namespace npantarhei.runtime.contract
{
    [Serializable]
    public class ParallelMethod : Attribute
    {
        private readonly string _threadPoolName;

        public ParallelMethod() : this("$$$parallelmethod$$$") { }
        public ParallelMethod(string threadPoolName)
        {
            _threadPoolName = threadPoolName;
        }

        public string ThreadPoolName { get { return _threadPoolName; } }
    }
}