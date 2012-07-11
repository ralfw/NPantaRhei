using System;
using System.Collections.Generic;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    internal class Serialize : IAsynchronizer
    {
        private readonly NotifyingMultiQueue<ScheduledTask<IMessage>> _messages;
        private readonly List<Wait_for_work<ScheduledTask<IMessage>>> _waitForWork;
        private readonly Func<IMessage, string> _getQueueNameFromMessage;
 

        public Serialize(Func<IMessage,string> getQueuenameFromMessage) : this(4, getQueuenameFromMessage) { }
        public Serialize(int numberOfThreads, Func<IMessage,string> getQueueNameFromMessage)
        {
            _messages = new NotifyingMultiQueue<ScheduledTask<IMessage>>();
            _getQueueNameFromMessage = getQueueNameFromMessage;

            _waitForWork = new List<Wait_for_work<ScheduledTask<IMessage>>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<ScheduledTask<IMessage>>(_messages,
                                               () =>
                                               {
                                                   ScheduledTask<IMessage> result;
                                                   var success = _messages.TryDequeue(Thread.CurrentThread.GetHashCode().ToString(),
                                                                                      out result);
                                                   return new Tuple<bool, ScheduledTask<IMessage>>(success, result);
                                               });
                wfw.Dequeued += _ => _.ContinueWith(_.Message);
                _waitForWork.Add(wfw);
            }
        }


        public void Process(IMessage message, Action<IMessage> continueWith)
        {
            var queueName = _getQueueNameFromMessage(message);
            _messages.Enqueue(new ScheduledTask<IMessage>()
                                  {
                                      Message = message,
                                      ContinueWith = continueWith
                                  }, 
                              queueName);
        }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }
    }
}

