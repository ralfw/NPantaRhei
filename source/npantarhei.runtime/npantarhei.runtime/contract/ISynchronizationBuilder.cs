using System;

namespace npantarhei.runtime.contract
{
    public interface ISynchronizationBuilder<T>
    {
        void Process(T t, Action<T> continueWith);
    }
}