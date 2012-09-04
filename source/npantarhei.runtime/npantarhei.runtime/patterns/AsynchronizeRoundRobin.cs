using System;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
    internal class AsynchronizeRoundRobin : IAsynchronizer
    {
        private readonly Parallelize _parallelize;

        internal AsynchronizeRoundRobin() { _parallelize = new Parallelize(1, new NotifyingPartionedQueue<ScheduledTask>()); }


        public void Process(IMessage message, Action<IMessage> continueWith) { _parallelize.Process(message, continueWith); }


        public void Start() { _parallelize.Start(); }
        public void Stop() { _parallelize.Stop(); }
    }
}