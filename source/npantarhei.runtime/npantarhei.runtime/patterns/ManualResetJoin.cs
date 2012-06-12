using System;
using System.Collections.Generic;
using System.Linq;

namespace npantarhei.runtime.patterns
{
    /*
     * in0: A1  A2         
     * in1:   B1    B2   B3  
     * in2:                   C1
     * out:                   (A1,B1,C1), (A2,B1,C1), (A2,B2,C1), (A2,B3,C1)
     */
    internal class ManualResetJoin
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


            public bool Is_ready()
            {
                return InputQueues.Count(q => q.Count > 0) == InputQueues.Count;
            }

            public bool Is_more_than_ready()
            {
                return Is_ready() && InputQueues.Any(q => q.Count > 1);
            }

            public List<object> Join_inputs()
            {
                return new List<object>(InputQueues.Select(q => q.Peek()));
            }

            public void Deplete()
            {
                InputQueues.First(q => q.Count > 1).Dequeue();
            }
        }


        private readonly int _numberOfInputs;
        private readonly Dictionary<Guid, JoinBucket> _buckets = new Dictionary<Guid,JoinBucket>();

        public ManualResetJoin(int numberOfInputs)
        {
            _numberOfInputs = numberOfInputs;
        }


        public void Reset(Guid correlationId)
        {
            _buckets.Remove(correlationId);
        }


        public void Process(int inputIndex, object inputData, Guid correlationId, Action<List<object>> continueOnJoin)
        {
            var bucket = Get_bucket(correlationId);

            Enqueue(bucket, inputIndex, inputData);
            Deplete_if_necessary(bucket, continueOnJoin);
            Join_if_ready(bucket, continueOnJoin);
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

        private void Enqueue(JoinBucket bucket, int inputIndex, object inputData)
        {
            if (bucket.Is_ready())
                bucket.InputQueues[inputIndex].Dequeue();
            bucket.InputQueues[inputIndex].Enqueue(inputData);
        }

        private void Deplete_if_necessary(JoinBucket bucket, Action<List<object>> continueOnJoin)
        {
            while (bucket.Is_more_than_ready())
            {
                continueOnJoin(bucket.Join_inputs());
                bucket.Deplete();
            }
        }

        private void Join_if_ready(JoinBucket bucket, Action<List<object>> continueOnJoin)
        {
            if (bucket.Is_ready())
                continueOnJoin(bucket.Join_inputs());
        }
    }
}