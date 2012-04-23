using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    /* Compiles a number of items of T from a stream into an IEnum<T>. The number of items to expect has to be set.
     * input:   .stream:     T
     *          .count:     int
     * output:  .*:         IEnum<T>
     */
    public class Gather<T> : AOperation
    {
        private List<T> _items; 
        private int? _numberOfItemsToExcept;

        public Gather(string name) : base(name) {}


        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            switch(input.Port.Name.ToLower())
            {
                case "stream":
                    if (_items == null) _items = new List<T>();
                    _items.Add((T)input.Data);
                    Complete_gathering(continueWith);
                    break;

                case "count":
                    _numberOfItemsToExcept = (int) input.Data;
                    Complete_gathering(continueWith);
                    break;

                default:
                    throw new ArgumentException("Input port not supported by Gather: " + input.Port.Name);
            }
        }


        private void Complete_gathering(Action<IMessage> continueWith)
        {
            if (_numberOfItemsToExcept == null || _items == null || _items.Count != _numberOfItemsToExcept) return;

            continueWith(new Message(base.Name, _items));
            _items = null;
        }
    }
}
