using System;
using System.Collections.Generic;
using System.Threading;
using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
    internal class Sequentialize<T>
    {
        private readonly NotifyingMultiQueue<T> _messages;
        private readonly List<Wait_for_work<T>> _waitForWork;


        public Sequentialize() : this(4) { }
        public Sequentialize(int numberOfThreads)
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
            Console.WriteLine("seq: {0}", portAfineMessage.Item2);
            _messages.Enqueue(portAfineMessage.Item1, portAfineMessage.Item2);
        }


        public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
        public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }


        public event Action<T> Dequeued;
    }
}

