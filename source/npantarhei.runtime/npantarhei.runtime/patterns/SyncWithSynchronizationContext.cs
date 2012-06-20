using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    internal class SyncWithSynchronizationContext : ISynchronizeWithContext
    {
        private readonly SynchronizationContext _synchronizationContext;

        public SyncWithSynchronizationContext() : this(SynchronizationContext.Current ?? new SynchronizationContext()) {}
        public SyncWithSynchronizationContext(SynchronizationContext synchronizationContext) { _synchronizationContext = synchronizationContext; }

        public void Process(IMessage t, Action<IMessage> continueWith)
        {
            _synchronizationContext.Send(state => continueWith(t), null);
        }
    }
}
