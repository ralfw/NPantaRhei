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
		private readonly Parallelize<T> _parallelize;
		
		internal Asynchronize() { 
			_parallelize = new Parallelize<T>(1);
			_parallelize.Dequeued += _ => Dequeued(_);
		}
		
		
		public void Enqueue(T message) { _parallelize.Enqueue(message); } 
		
		
		public void Start() { _parallelize.Start(); }
		public void Stop() { _parallelize.Stop(); }
			
		
		public event Action<T> Dequeued;
	}


    internal class Asynchronize2<T> : IOperationImplementationWrapper<T>
    {
        private readonly Parallelize2<T> _parallelize;

        internal Asynchronize2() { _parallelize = new Parallelize2<T>(1); }


        public void Process(T message, Action<T> continueWith) { _parallelize.Process(message, continueWith); }


        public void Start() { _parallelize.Start(); }
        public void Stop() { _parallelize.Stop(); }
    }
}

