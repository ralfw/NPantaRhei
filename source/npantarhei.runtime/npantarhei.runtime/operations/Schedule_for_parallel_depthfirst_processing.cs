using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    public class Schedule_for_parallel_depthfirst_processing : ISchedulingStrategy
    {
        private readonly IAsynchronizer _async = new Parallelize();


        public void ProcessExternalMessage(IMessage message)
        {
            _async.Process(message, Result);
        }

        public void ProcessInternalMessage(IMessage message)
        {
            Result(message);
        }


        public void Start() { _async.Start(); }
        public void Stop() { _async.Stop(); }


        public event Action<IMessage> Result;
    }
}