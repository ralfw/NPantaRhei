using System;
using System.Collections.Concurrent;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.data
{
    class AsynchronizerCache
    {
        private readonly ConcurrentDictionary<string, IAsynchronizer> _asyncers = new ConcurrentDictionary<string, IAsynchronizer>();

        public IAsynchronizer Get(string name, Func<IAsynchronizer> factory)
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