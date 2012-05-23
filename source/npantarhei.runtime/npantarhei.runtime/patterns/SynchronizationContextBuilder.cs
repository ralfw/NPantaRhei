using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    internal class SynchronizationContextBuilder<T> : ISynchronizationBuilder<T>
    {
        private readonly SynchronizationContext _synchronizationContext;

        public SynchronizationContextBuilder() : this(SynchronizationContext.Current ?? new SynchronizationContext()) {}
        public SynchronizationContextBuilder(SynchronizationContext synchronizationContext) { _synchronizationContext = synchronizationContext; }

        public void Process(T t, Action<T> continueWith)
        {
            _synchronizationContext.Send(state => continueWith(t), null);
        }
    }
}
