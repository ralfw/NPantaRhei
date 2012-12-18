using System;
using System.Collections.Generic;
using System.Linq;

namespace npantarhei.distribution.translators
{
    class CorrelationCache<T>
    {
        internal struct Element
        {
            public Guid CorrelationId;
            public T Data;
            public DateTime ExpiresAt;
        }


        internal List<Element> _cache = new List<Element>();

        private const int GC_FREQUENCY = 1000;
        private const int INITIAL_LIFESPAN_SEC = 60;
        private int _gcCounter;


        public void Add(Guid correlationId, T data)
        {
            lock (_cache)
            {
                if (++_gcCounter % GC_FREQUENCY == 0) CollectGarbage();
                _cache.Add(new Element { CorrelationId = correlationId, Data = data, ExpiresAt = DateTime.Now.AddSeconds(INITIAL_LIFESPAN_SEC) });
            }
        }


        public T Get(Guid correlationId)
        {
            lock (_cache)
            {
                //TODO: extend life span with use
                return _cache.First(e => e.CorrelationId == correlationId).Data;
            }
        }


        internal void CollectGarbage()
        {
            var indexesOfExpired = _cache.Select((e,i) => new {e.ExpiresAt, Index = i})
                                         .Where(e => e.ExpiresAt <= DateTime.Now)
                                         .Select(e => e.Index)
                                         .Reverse()
                                         .ToArray();
            foreach (var i in indexesOfExpired)
                _cache.RemoveAt(i);
        }
    }
}
