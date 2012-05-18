using System;

namespace npantarhei.runtime.contract
{
    public interface ISynchronize<T>
    {
        void Process(T t, Action<T> continueWith);
    }
}