using System;
using System.Collections.Generic;
using System.Threading;

namespace npantarhei.runtime.data
{
	internal class NotifyingSingleQueue<T> : INotifyingResource
	{
		private readonly Queue<T> _messages;
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
		private readonly ManualResetEvent _signal = new ManualResetEvent(false);
		
		
		public NotifyingSingleQueue() : this(new Queue<T>()) {}
		internal NotifyingSingleQueue(Queue<T> messages)
		{
			_messages = messages;
		}
		
		
		public void Enqueue(T message)
		{
			_lock.AcquireWriterLock(500);
			try
			{
				_messages.Enqueue(message);
				_signal.Set();
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
		}
		
		
		public bool TryDequeue(out T message)
		{
			_lock.AcquireWriterLock(500);
			try
			{
				if (_messages.Count > 0)
				{
					message = _messages.Dequeue();
					_signal.Reset();
					return true;
				}
				else
				{
					message = default(T);
					_signal.Reset();
					return false;
				}
			}
			finally
			{
				_lock.ReleaseWriterLock();
			}
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

