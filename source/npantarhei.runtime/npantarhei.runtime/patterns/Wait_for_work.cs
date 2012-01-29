using System;
using System.Threading;

using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
	internal class Wait_for_work<T>
	{
		private readonly INotifyingResource _resource;
	    private readonly Func<Tuple<bool, T>> _getWorkFromResource; 
		
		private Thread _th;
		internal bool _running;


        public Wait_for_work(INotifyingResource resource, Func<Tuple<bool, T>> getWorkFromResource)
		{
		    _resource = resource;
		    _getWorkFromResource = getWorkFromResource;
		}
				
		
		public void Start() { Start(Dequeue_messages); }
		internal void Start(Action<Func<bool>> backgroundTask)
		{
			if (_th == null)
			{
				_th = new Thread(() => backgroundTask(() => _running));
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
		
		
		internal void Dequeue_messages(Func<bool> isRunning)
		{
			do
			{
			    var work = _getWorkFromResource();
				if (work.Item1) 
					Dequeued(work.Item2);
				else
					_resource.Wait(Timeout.Infinite);
			} while(isRunning());
		}
	}
}

