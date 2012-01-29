using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using npantarhei.runtime.contract;
using npantarhei.runtime.data;

namespace npantarhei.runtime.patterns
{
	internal class Asynchronize<T>
	{
		private Parallelize<T> _parallelize;
		
		internal Asynchronize() { 
			_parallelize = new Parallelize<T>(1);
			_parallelize.Dequeued += _ => Dequeued(_);
		}
		
		
		public void Enqueue(T message) { _parallelize.Enqueue(message); } 
		
		
		public void Start() { _parallelize.Start(); }
		public void Stop() { _parallelize.Stop(); }
			
		
		public event Action<T> Dequeued;
	}
}

