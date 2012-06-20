using System;
using System.Windows.Threading;
using npantarhei.runtime.contract;

namespace todo.app
{
    public class WpfSynchronize: ISynchronizationBuilder<IMessage>
    {
        private readonly Dispatcher dispatcher;

        public WpfSynchronize() {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Process(IMessage t, Action<IMessage> continueWith) {
            dispatcher.Invoke(new Action(() => continueWith(t)));
            
        }
    }
}