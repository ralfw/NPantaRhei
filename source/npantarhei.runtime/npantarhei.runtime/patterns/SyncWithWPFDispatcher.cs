using System;
using System.Windows.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    public class SyncWithWPFDispatcher: ISynchronizeWithContext
    {
        private readonly Dispatcher dispatcher;

        public SyncWithWPFDispatcher() {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Process(IMessage t, Action<IMessage> continueWith) {
            dispatcher.Invoke(new Action(() => continueWith(t)));
        }
    }
}