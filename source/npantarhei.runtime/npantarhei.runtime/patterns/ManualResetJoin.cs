using System;
using System.Collections.Generic;
using System.Linq;

namespace npantarhei.runtime.patterns
{
    /*
     * in0: A1  A2                          A3         A4
     * in1:   B1               B2                          B3           B4
     * in2:       C1                      C2             C3           C4
     * out:       (A1,B1,C1)   (A2,B2,C1)   (A3,B2,C2)     (A3,B3,C3)   (A4,B4,C4)
     */
    internal class ManualResetJoin
    {
        private readonly int _numberOfInputs;
        private List<Queue<object>> _inputQueues;

        public ManualResetJoin(int numberOfInputs)
        {
            _numberOfInputs = numberOfInputs;
            Reset();
        }


        public void Process(int inputIndex, object inputData, Action<List<object>> continueOnJoin)
        {
            lock (this)
            {
                _inputQueues[inputIndex].Enqueue(inputData);

                while (Is_ready_to_join())
                    continueOnJoin(Join_inputs(inputIndex));
            }
        }


        public void Reset()
        {
            lock(this)
            {
                _inputQueues = new List<Queue<object>>();
                for (var i = 0; i < _numberOfInputs; i++)
                    _inputQueues.Add(new Queue<object>());
            }
        }


        private bool Is_ready_to_join()
        {
            return _inputQueues.Count(q => q.Count > 0) == _inputQueues.Count;
        }

        private List<object> Join_inputs(int currentIndex)
        {
            return new List<object>(_inputQueues.Select((q,i) => i==currentIndex ? q.Peek() : q.Dequeue()));
        }
    }
}