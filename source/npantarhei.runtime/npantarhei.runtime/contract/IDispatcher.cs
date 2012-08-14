using System;

namespace npantarhei.runtime.contract
{
    public interface IDispatcher
    {
        void Process(IMessage t, Action<IMessage> continueWith);
        void Process(Action continueWith);
    }
}