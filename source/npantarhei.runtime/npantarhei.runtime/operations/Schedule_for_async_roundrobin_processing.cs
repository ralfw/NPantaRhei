using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.flows;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    public class Schedule_for_async_roundrobin_processing : ISchedulingStrategy
    {
        private readonly IAsynchronizer _async = new AsynchronizeRoundRobin();


        public void ProcessExternalMessage(IMessage message)
        {
            _async.Process(message, Result);
        }

        public void ProcessInternalMessage(IMessage message)
        {
            ProcessExternalMessage(message);
        }


        public void Start() { _async.Start(); }
        public void Stop() { _async.Stop(); }


        public event Action<IMessage> Result;
    }
}
