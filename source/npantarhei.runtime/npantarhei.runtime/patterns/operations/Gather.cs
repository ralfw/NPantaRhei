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
        private class GatherBucket
        {
            public readonly List<T> Items = new List<T>();
            public int? NumberOfItemsToExpect;

            public bool IsFull { get { return NumberOfItemsToExpect != null && Items.Count >= NumberOfItemsToExpect; }}
        }

        private readonly Dictionary<Guid, GatherBucket> _buckets = new Dictionary<Guid, GatherBucket>(); 


        public Gather() : this("gather") {}
        public Gather(string name) : base(name) {}


        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            GatherBucket bucket;
            if (!_buckets.TryGetValue(input.CorrelationId, out bucket))
            {
                bucket = new GatherBucket();
                _buckets.Add(input.CorrelationId, bucket);
            }

            switch(input.Port.Name.ToLower())
            {
                case "stream":
                    bucket.Items.Add((T)input.Data);
                    Complete_gathering(input.CorrelationId, continueWith);
                    break;

                case "count":
                    bucket.NumberOfItemsToExpect = (int)input.Data;
                    Complete_gathering(input.CorrelationId, continueWith);
                    break;

                default:
                    throw new ArgumentException("Input port not supported by Gather: " + input.Port.Name);
            }
        }


        private void Complete_gathering(Guid correlationId, Action<IMessage> continueWith)
        {
            var bucket = _buckets[correlationId];
            if (!bucket.IsFull) return;

            continueWith(new Message(base.Name, bucket.Items.Take((int)bucket.NumberOfItemsToExpect).ToArray(), correlationId));
            bucket.Items.RemoveRange(0, (int)bucket.NumberOfItemsToExpect); // remove items gathered from bucket
            if (bucket.Items.Count == 0) _buckets.Remove(correlationId);    // remove bucket if empty (en passent bucket GC)
        }
    }
}
