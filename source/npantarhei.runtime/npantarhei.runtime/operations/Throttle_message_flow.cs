using System;
using System.Threading;
using npantarhei.runtime.contract;

namespace npantarhei.runtime.flows
{
    internal class Throttle_message_flow
    {
        public void Process(IMessage msg)
        {
            if (_delayMilliseconds > 0) Thread.Sleep(_delayMilliseconds);
            Continue(msg);
        }

        private int _delayMilliseconds;
        public void Delay(int milliseconds)
        {
            _delayMilliseconds = milliseconds;
        }

        public event Action<IMessage> Continue;
    }
}