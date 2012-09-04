using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.flows;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    class Schedule_for_async_depthfirst_processing : IScheduler
    {
        private readonly IAsynchronizer _async = new AsynchronizeFIFO();


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
