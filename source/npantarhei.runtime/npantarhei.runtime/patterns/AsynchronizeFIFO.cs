using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    internal class AsynchronizeFIFO : IAsynchronizer
    {
        private readonly Parallelize _parallelize;

        internal AsynchronizeFIFO() { _parallelize = new Parallelize(1); }


        public void Process(IMessage message, Action<IMessage> continueWith) { _parallelize.Process(message, continueWith); }


        public void Start() { _parallelize.Start(); }
        public void Stop() { _parallelize.Stop(); }
    }
}

