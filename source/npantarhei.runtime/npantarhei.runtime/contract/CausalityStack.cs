using System.Collections.Generic;
using System.Linq;

namespace npantarhei.runtime.contract
{
    public class CausalityStack
    {
        private Stack<Causality> _causalities;

        public CausalityStack() : this(new Stack<Causality>()) {}
        internal CausalityStack(Stack<Causality> causalities) { _causalities = causalities; }


        public void Push(IPort exceptionHandler)
        {
            _causalities.Push(new Causality(exceptionHandler));
        }

        public void Pop()
        {
            _causalities.Pop();
        }

        public Causality Peek()
        {
            return _causalities.Peek();
        }

        public bool IsEmpty { get { return _causalities.Count == 0;  } }


        public CausalityStack Copy()
        {
            return new CausalityStack {_causalities = new Stack<Causality>(this._causalities.Reverse())};
        }
    }


    public class Causality
    {
        public Causality(IPort exceptionHandler)
        {
            Port = exceptionHandler;
        }

        public IPort Port { get; private set; }
    }
}