using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace npantarhei.runtime.data
{
    internal class NotifyingMultiQueue<T> : INotifyingResource
    {
        readonly List<NamedQueue> _queues = new List<NamedQueue>(); 
        readonly Dictionary<string, string> _readLocks = new Dictionary<string,string>();
        readonly ReaderWriterLock _lock = new ReaderWriterLock();
        readonly ManualResetEvent _signal = new ManualResetEvent(false);

        class NamedQueue
        {
            public string Name;
            public Queue<T> Queue;
        }


        public void Enqueue(T message, string queueName)
        {
            _lock.AcquireWriterLock(500);

            var namedQueue = _queues.FirstOrDefault(nq => nq.Name == queueName);
            if (namedQueue == null)
            {
                namedQueue = new NamedQueue {Name = queueName, Queue = new Queue<T>()};
                _queues.Add(namedQueue);
            }
            namedQueue.Queue.Enqueue(message);
            if (Queue_not_blocked(namedQueue)) _signal.Set();

            _lock.ReleaseWriterLock();
        }


        public bool TryDequeue(string workerId, out T message)
        {
            _lock.AcquireWriterLock(500);
            try
            {
                message = default(T);

                Free_queue_locked_for_worker(workerId);

                NamedQueue namedQueue = null;
                for (var i = 0; i < _queues.Count(); i++)
                {
                    namedQueue = Get_next_queue();
                    if (Queue_ready_for_dequeue(namedQueue)) break;
                    namedQueue = null;
                }
                if (namedQueue == null)
                {
                    _signal.Reset();
                    return false;
                }

                message = namedQueue.Queue.Dequeue();
                Lock_queue_for_worker(workerId, namedQueue);

                _signal.Reset();
                return true;
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }


        private void Free_queue_locked_for_worker(string workerId)
        {
            if (_readLocks.ContainsValue(workerId))
                _readLocks.Remove(_readLocks.Where(kvp => kvp.Value == workerId).Select(kvp => kvp.Key).First());
        }

        private NamedQueue Get_next_queue()
        {
            var namedQueue = _queues[0];
            _queues.RemoveAt(0);
            _queues.Add(namedQueue);
            return namedQueue;
        }
        
        private bool Queue_ready_for_dequeue(NamedQueue namedQueue)
        {
            return Queue_not_blocked(namedQueue) && namedQueue.Queue.Any();
        }

        private bool Queue_not_blocked(NamedQueue namedQueue)
        {
            return !_readLocks.ContainsKey(namedQueue.Name);
        }

        private void Lock_queue_for_worker(string workerId,NamedQueue namedQueue)
        {
            _readLocks[namedQueue.Name] = workerId;
        }


        public void Notify()
        {
            _lock.AcquireReaderLock(500);
            _signal.Set();
            _lock.ReleaseReaderLock();
        }


        public bool Wait(int milliseconds)
        {
            return _signal.WaitOne(milliseconds);
        }
    }
}
