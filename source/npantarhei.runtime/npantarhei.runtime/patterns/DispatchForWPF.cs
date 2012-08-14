using System;
using System.Windows.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.patterns
{
    public class DispatchForWPF: IDispatcher
    {
        private readonly Dispatcher dispatcher;

        public DispatchForWPF() {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Process(IMessage t, Action<IMessage> continueWith) {
            Process(() => continueWith(t));
        }

        public void Process(Action continueWith)
        {
            dispatcher.Invoke(continueWith);
        }
    }
}