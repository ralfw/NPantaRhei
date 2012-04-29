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
        private readonly List<Queue<object>> _inputQueues;
 
        public AutoResetJoin(int numberOfInputs)
        {
            _inputQueues = new List<Queue<object>>();
            for (var i = 0; i < numberOfInputs; i++)
                _inputQueues.Add(new Queue<object>());
        }


        public void Process(int inputIndex, object inputData, Action<List<object>> continueOnJoin)
        {
            lock (this)
            {
                _inputQueues[inputIndex].Enqueue(inputData);

                if (Is_ready_to_join())
                    continueOnJoin(Join_inputs());
            }
        }


        private bool Is_ready_to_join()
        {
            return _inputQueues.Count(q => q.Count > 0) == _inputQueues.Count;
        }

        private List<object> Join_inputs()
        {
            return new List<object>(_inputQueues.Select(q => q.Dequeue()));
        }
    }
}