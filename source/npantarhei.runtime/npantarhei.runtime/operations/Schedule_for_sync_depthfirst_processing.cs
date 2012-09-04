using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.flows;
using npantarhei.runtime.patterns;

namespace npantarhei.runtime.operations
{
    public class Schedule_for_sync_depthfirst_processing : IScheduler
    {
        public void ProcessExternalMessage(IMessage message)
        {
            Result(message);
        }

        public void ProcessInternalMessage(IMessage message)
        {
            ProcessExternalMessage(message);
        }


        public void Start() { }
        public void Stop() { }


        public event Action<IMessage> Result;
    }
}
