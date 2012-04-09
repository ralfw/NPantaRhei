using System;
using System.Collections.Generic;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
    internal class Serialize<T>
    {
        private readonly NotifyingMultiQueue<T> _messages;
        private readonly List<Wait_for_work<T>> _waitForWork;


        public Serialize() : this(4) { }
        public Serialize(int numberOfThreads)
        {
            _messages = new NotifyingMultiQueue<T>();

            _waitForWork = new List<Wait_for_work<T>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<T>(_messages, 
                                               () => {
                                                         T result;
                                                         var success = _messages.TryDequeue(Thread.CurrentThread.GetHashCode().ToString(),
                                                                                            out result);
                                                         return new Tuple<bool, T>(success, result);
                                                     });
                wfw.Dequeued += _ => Dequeued(_);
                _waitForWork.Add(wfw);
            }
        }


        public void Enqueue(Tuple<T, string> portAfineMessage) 
        { 
            _messages.Enqueue(portAfineMessage.Item1, portAfineMessage.Item2);
        }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }


        public event Action<T> Dequeued;
    }


    internal class Serialize2<T> : IOperationImplementationWrapper<T>
    {
        private readonly NotifyingMultiQueue<ScheduledTask<T>> _messages;
        private readonly List<Wait_for_work<ScheduledTask<T>>> _waitForWork;
        private readonly Func<T, string> _getQueueNameFromMessage;
 

        public Serialize2(Func<T,string> getQueuenameFromMessage) : this(4, getQueuenameFromMessage) { }
        public Serialize2(int numberOfThreads, Func<T,string> getQueueNameFromMessage)
        {
            _messages = new NotifyingMultiQueue<ScheduledTask<T>>();
            _getQueueNameFromMessage = getQueueNameFromMessage;

            _waitForWork = new List<Wait_for_work<ScheduledTask<T>>>();
            for (var i = 0; i < numberOfThreads; i++)
            {
                var wfw = new Wait_for_work<ScheduledTask<T>>(_messages,
                                               () =>
                                               {
                                                   ScheduledTask<T> result;
                                                   var success = _messages.TryDequeue(Thread.CurrentThread.GetHashCode().ToString(),
                                                                                      out result);
                                                   return new Tuple<bool, ScheduledTask<T>>(success, result);
                                               });
                wfw.Dequeued += _ => _.ContinueWith(_.Message);
                _waitForWork.Add(wfw);
            }
        }


        public void Process(T message, Action<T> continueWith)
        {
            var queueName = _getQueueNameFromMessage(message);
            _messages.Enqueue(new ScheduledTask<T>()
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

