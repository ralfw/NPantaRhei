using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    internal class DispatchWithSynchronizationContext : IDispatcher
    {
        private readonly SynchronizationContext _synchronizationContext;

        public DispatchWithSynchronizationContext() : this(SynchronizationContext.Current ?? new SynchronizationContext()) {}
        public DispatchWithSynchronizationContext(SynchronizationContext synchronizationContext) { _synchronizationContext = synchronizationContext; }

        public void Process(IMessage t, Action<IMessage> continueWith)
        {
            Process(() => continueWith(t));
        }

        public void Process(Action continueWith)
        {
            _synchronizationContext.Send(state => continueWith(), null);
        }
    }
}
