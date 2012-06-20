using System;

namespace npantarhei.runtime.contract
{
    public interface ISynchronizeWithContext
    {
        void Process(IMessage t, Action<IMessage> continueWith);
    }
}