using System;
using System.Collections.Generic;
using System.Linq;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
	internal class Parallelize<T>
	{
		private readonly NotifyingSingleQueue<T> _messages;
		private readonly List<Wait_for_work<T>> _waitForWork;
		
		
		public Parallelize() : this(4) {}
		public Parallelize(int numberOfThreads) { 
			_messages = new NotifyingSingleQueue<T>();
			
			_waitForWork = new List<Wait_for_work<T>>();
			for(var i=0; i<numberOfThreads; i++) {
				var wfw = new Wait_for_work<T>(_messages, 
                                               () => {
				                                         T result;
				                                         var success = _messages.TryDequeue(out result);
                                                         return new Tuple<bool, T>(success, result);
				                                     });
				wfw.Dequeued += _ => Dequeued(_);
				_waitForWork.Add(wfw);
			}
		}
		
		
		public void Enqueue(T message) { _messages.Enqueue(message); } 
		
		
		public void Start() { _waitForWork.ForEach(wfw => wfw.Start()); }
		public void Stop() { _waitForWork.ForEach(wfw => wfw.Stop()); }
			
		
		public event Action<T> Dequeued;
	}



    class ScheduledTask<T>
    {
        public T Message;
        public Action<T> ContinueWith;
    }


    internal class Parallelize2<T> : IOperationImplementationWrapper<T>
    {
        private readonly NotifyingSingleQueue<ScheduledTask<T>> _messages;
        private readonly List<Wait_for_work<ScheduledTask<T>>> _waitForWork;


        public Parallelize2() : this(4) { }
        public Parallelize2(int numberOfThreads)
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

