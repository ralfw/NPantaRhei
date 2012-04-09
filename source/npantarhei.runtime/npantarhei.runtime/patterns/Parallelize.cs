using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    internal class Parallelize<T> : IAsynchronizer<T>
    {
        private readonly NotifyingSingleQueue<ScheduledTask<T>> _messages;
        private readonly List<Wait_for_work<ScheduledTask<T>>> _waitForWork;


        public Parallelize() : this(4) { }
        public Parallelize(int numberOfThreads)
        {
            _messages = new NotifyingSingleQueue<ScheduledTask<T>>();

            _waitForWork = new List<Wait_for_work<ScheduledTask<T>>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<ScheduledTask<T>>(_messages,
                                               () =>
                                               {
                                                   ScheduledTask<T> result;
                                                   var success = _messages.TryDequeue(out result);
                                                   return new Tuple<bool, ScheduledTask<T>>(success, result);
                                               });
                wfw.Dequeued += _ => _.ContinueWith(_.Message);
                _waitForWork.Add(wfw);
            }
        }


        public void Process(T message, Action<T> continueWith) { _messages.Enqueue(new ScheduledTask<T>()
                                                                                       {
                                                                                           Message = message,
                                                                                           ContinueWith = continueWith
                                                                                       }); }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }
    }
}

