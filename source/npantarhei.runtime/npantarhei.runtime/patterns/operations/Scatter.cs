using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    /* Converts a IEnum<T> to a stream of items of T to be processed in parallel.
     * Input:   .*:         IEnum<T>
     * Output:  .stream:    T
     *          .count      int
     */
    public class Scatter<T> : AOperation
    {
        public Scatter() : this("scatter") {} 
        public Scatter(string name) : base(name) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            var items = (IEnumerable<T>) input.Data;
            var count = 0;
            foreach (var item in items)
            {
                continueWith(new Message(this.Name + ".stream", item, input.CorrelationId));
                count++;
            }
            continueWith(new Message(this.Name + ".count", count, input.CorrelationId));
        }
    }
}
