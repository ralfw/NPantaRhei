using System;

namespace npantarhei.runtime.contract
{
    public interface ISynchronizationBuilder
    {
        void Process(IMessage t, Action<IMessage> continueWith);
    }
}