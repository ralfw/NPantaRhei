using System;
using System.Threading;

using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
	internal class Wait_for_work_on_queue<T>
	{
	    internal delegate bool TryDequeueDelegate<S>(out S message);

		private INotifyingResource _resource;
		
		private Thread _th;
		internal bool _running;


        public Wait_for_work_on_queue(INotifyingResource resource) { _resource = resource; }
				
		
		public void Start(TryDequeueDelegate<T> trydequeue) { Start(Dequeue_messages, trydequeue); }
		internal void Start(Action<TryDequeueDelegate<T>, Func<bool>> backgroundTask, TryDequeueDelegate<T> trydequeue)
		{
			if (_th == null)
			{
				_th = new Thread(() => backgroundTask(trydequeue, () => _running));
				_th.IsBackground = true;
				_running = true;
				_th.Start();
			}
		}
		
		public void Stop() { 
			_running = false; 
			_resource.Notify();
			if (_th != null) _th.Join();
		}
		
		
		public event Action<T> Dequeued;
		
		
		internal void Dequeue_messages(TryDequeueDelegate<T> trydequeue, Func<bool> isRunning)
		{
			var msg = default(T);
			do
			{
				if (trydequeue(out msg)) 
					Dequeued(msg);
				else
					_resource.Wait(Timeout.Infinite);
			} while(isRunning());
		}
	}
}

