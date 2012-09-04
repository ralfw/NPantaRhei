using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    internal class Parallelize : IAsynchronizer
    {
        private readonly IConcurrentQueue<ScheduledTask> _messages;
        private readonly List<Wait_for_work<ScheduledTask>> _waitForWork;


        public Parallelize() : this(4) { }
        public Parallelize(int numberOfThreads) : this(numberOfThreads, new NotifyingSingleQueue<ScheduledTask>()) { }
        public Parallelize(int numberOfThreads, IConcurrentQueue<ScheduledTask> messages)
        {
            _messages = messages;

            _waitForWork = new List<Wait_for_work<ScheduledTask>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<ScheduledTask>(_messages,
                                               () =>
                                               {
                                                   ScheduledTask result;
                                                   var success = _messages.TryDequeue(out result);
                                                   return new Tuple<bool, ScheduledTask>(success, result);
                                               });
                wfw.Dequeued += _ => _.ContinueWith(_.Message);
                _waitForWork.Add(wfw);
            }
        }


        public void Process(IMessage message, Action<IMessage> continueWith) { _messages.Enqueue(message.Priority, 
                                                                                                 new ScheduledTask()
                                                                                                       {
                                                                                                           Message = message,
                                                                                                           ContinueWith = continueWith
                                                                                                       }); }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }
    }
}

