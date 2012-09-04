using System;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.contract
{
    public interface IScheduler
    {
        void ProcessExternalMessage(IMessage message);
        void ProcessInternalMessage(IMessage message);

        void Start();
        void Stop();

        event Action<IMessage> Result;
    }
}