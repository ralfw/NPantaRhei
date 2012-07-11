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
        private readonly NotifyingSingleQueue<ScheduledTask<IMessage>> _messages;
        private readonly List<Wait_for_work<ScheduledTask<IMessage>>> _waitForWork;


        public Parallelize() : this(4) { }
        public Parallelize(int numberOfThreads)
        {
            _messages = new NotifyingSingleQueue<ScheduledTask<IMessage>>();

            _waitForWork = new List<Wait_for_work<ScheduledTask<IMessage>>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<ScheduledTask<IMessage>>(_messages,
                                               () =>
                                               {
                                                   ScheduledTask<IMessage> result;
                                                   var success = _messages.TryDequeue(out result);
                                                   return new Tuple<bool, ScheduledTask<IMessage>>(success, result);
                                               });
                wfw.Dequeued += _ => _.ContinueWith(_.Message);
                _waitForWork.Add(wfw);
            }
        }


        public void Process(IMessage message, Action<IMessage> continueWith) { _messages.Enqueue(message.Priority, 
                                                                                                 new ScheduledTask<IMessage>()
                                                                                                       {
                                                                                                           Message = message,
                                                                                                           ContinueWith = continueWith
                                                                                                       }); }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }
    }
}

