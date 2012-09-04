using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace npantarhei.runtime.data
{
	internal class NotifyingPartionedQueue<T> : IConcurrentQueue<T> where T : IPartionable
	{
		public class PartitionRing
		{
			private readonly Dictionary<string, PriorityQueue<T>> _partitions = new Dictionary<string, PriorityQueue<T>>();
			private readonly List<PriorityQueue<T>> _ring = new List<PriorityQueue<T>>();

			public PriorityQueue<T> Create(string partitionName)
			{
				PriorityQueue<T> partition;
				if (!_partitions.TryGetValue(partitionName, out partition))
				{
					partition = new PriorityQueue<T>();
					_partitions.Add(partitionName, partition);
					_ring.Add(partition);
				}
				return partition;
			}


			public PriorityQueue<T> Next()
			{
				var partition = _ring.First();
				if (_ring.Count > 1)
				{
					_ring.RemoveAt(0);
					_ring.Add(partition);
				}

				return partition;
			}


			public void Remove(PriorityQueue<T> partition)
			{
				var partitionName = _partitions.Where(kvp => kvp.Value == partition).Select(kvp => kvp.Key).First();
				_ring.Remove(partition);
				_partitions.Remove(partitionName);
			}


			public int Count { get { return _partitions.Count; } }
		}


		private readonly PartitionRing _partitionRing;
		private readonly ReaderWriterLock _lock = new ReaderWriterLock();
		private readonly ManualResetEvent _signal = new ManualResetEvent(false);
		
		
		internal NotifyingPartionedQueue()
		{
			_partitionRing = new PartitionRing();
		}
		
		
		public void Enqueue(int priority, T message)
		{
			_lock.AcquireWriterLock(500);
			try
			{
				var partition = _partitionRing.Create(message.Partition);

				partition.Enqueue(priority, message);
				
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
				if (_partitionRing.Count > 0)
				{
					var partition = _partitionRing.Next();

					message = partition.Dequeue();

					if (partition.Count == 0) _partitionRing.Remove(partition);

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