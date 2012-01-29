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
}

