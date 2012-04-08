using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace npantarhei.runtime.patterns
{
    internal class Synchronize<T>
    {
        private readonly SynchronizationContext _synchronizationContext;

        public Synchronize() : this(SynchronizationContext.Current ?? new SynchronizationContext()) {}
        public Synchronize(SynchronizationContext synchronizationContext) { _synchronizationContext = synchronizationContext; }

        public void Process(T t, Action<T> continueWith)
        {
            _synchronizationContext.Send(state => continueWith(t), null);
        }
    }
}
