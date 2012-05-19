using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.contract
{
    public class FlowStack
    {
        private Stack<string> _flownames;

        public FlowStack() : this(new Stack<string>()) { }
        internal FlowStack(Stack<string> flownames) { _flownames = flownames; }


        public void Push(string flowname)
        {
            _flownames.Push(flowname);
        }

        public string Pop()
        {
            return _flownames.Pop();
        }


        public bool IsEmpty { get { return _flownames.Count == 0; } }

        public int Depth { get { return _flownames.Count; } }


        public FlowStack Copy()
        {
            return new FlowStack { _flownames = new Stack<string>(_flownames.Reverse()) };
        }
    }
}
