using System;
using System.Collections.Generic;
using System.Linq;

namespace npantarhei.runtime.patterns
{
    /*
     * in0: A1  A2
     * in1:   B1               B2
     * in2:       C1         C2
     * out:       (A1,B1,C1)   (A2,B2,C2)
     */
    internal class AutoResetJoin
    {
        class JoinBucket
        {
            public JoinBucket(int numberOfInputs)
            {
                InputQueues = new List<Queue<object>>();
                for (var i = 0; i < numberOfInputs; i++)
                    InputQueues.Add(new Queue<object>());
            }

            public readonly List<Queue<object>> InputQueues;


            public bool Is_ready_to_join()
            {
                return InputQueues.Count(q => q.Count > 0) == InputQueues.Count;
            }

            public bool Is_empty()
            {
                return InputQueues.Count(q => q.Count > 0) == 0;
            }

            public List<object> Join_inputs()
            {
                return new List<object>(InputQueues.Select(q => q.Dequeue()));
            }
        }


        private readonly int _numberOfInputs;
        private readonly Dictionary<Guid, JoinBucket> _buckets = new Dictionary<Guid,JoinBucket>(); 
 
        public AutoResetJoin(int numberOfInputs)
        {
            _numberOfInputs = numberOfInputs;
        }


        public void Process(int inputIndex, object inputData, Guid correlationId, Action<List<object>> continueOnJoin)
        {
            var bucket = Get_bucket(correlationId);

            bucket.InputQueues[inputIndex].Enqueue(inputData);

            if (bucket.Is_ready_to_join())
            {
                continueOnJoin(bucket.Join_inputs());
                if (bucket.Is_empty()) _buckets.Remove(correlationId);
            }
        }

        private JoinBucket Get_bucket(Guid correlationId)
        {
            JoinBucket bucket = null;
            if (!_buckets.TryGetValue(correlationId, out bucket))
            {
                bucket = new JoinBucket(_numberOfInputs);
                _buckets[correlationId] = bucket;
            }
            return bucket;
        }
    }
}