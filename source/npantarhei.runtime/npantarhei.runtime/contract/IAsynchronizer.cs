using System;

namespace npantarhei.runtime.contract
{
    public interface IAsynchronizer
    {
        void Process(IMessage message, Action<IMessage> continueWith);

        void Start();
        void Stop();
    }
}