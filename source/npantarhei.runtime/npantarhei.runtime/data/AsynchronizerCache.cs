using System;
using System.Collections.Concurrent;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.data
{
    class AsynchronizerCache
    {
        private readonly ConcurrentDictionary<string, IScheduler> _asyncers = new ConcurrentDictionary<string, IScheduler>();

        public IScheduler Get(string name, Func<IScheduler> factory)
        {
            return _asyncers.GetOrAdd(name, key =>
                                                {
                                                    var asyncer = factory();
                                                    asyncer.Start();
                                                    return asyncer;
                                                });
        }    
    }
}