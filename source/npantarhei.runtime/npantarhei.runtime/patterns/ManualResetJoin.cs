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
        private readonly int _numberOfInputs;
        private List<Queue<object>> _inputQueues;

        public ManualResetJoin(int numberOfInputs)
        {
            _numberOfInputs = numberOfInputs;
            Reset();
        }


        public void Reset()
        {
            lock (this)
            {
                _inputQueues = new List<Queue<object>>();
                for (var i = 0; i < _numberOfInputs; i++)
                    _inputQueues.Add(new Queue<object>());
            }
        }


        public void Process(int inputIndex, object inputData, Action<List<object>> continueOnJoin)
        {
            lock (this)
            {
                Enqueue(inputIndex, inputData);
                Deplete_if_necessary(continueOnJoin);
                Join_if_ready(continueOnJoin);
            }
        }


        private void Enqueue(int inputIndex, object inputData)
        {
            if (Is_ready())
                _inputQueues[inputIndex].Dequeue();
            _inputQueues[inputIndex].Enqueue(inputData);
        }

        private void Deplete_if_necessary(Action<List<object>> continueOnJoin)
        {
            while (Is_more_than_ready())
            {
                continueOnJoin(Join_inputs());
                Deplete();
            }
        }

        private void Join_if_ready(Action<List<object>> continueOnJoin)
        {
            if (Is_ready())
                continueOnJoin(Join_inputs());
        }


        private bool Is_ready()
        {
            return _inputQueues.Count(q => q.Count > 0) == _inputQueues.Count;
        }

        private bool Is_more_than_ready()
        {
            return Is_ready() && _inputQueues.Any(q => q.Count > 1);
        }


        private List<object> Join_inputs()
        {
            return new List<object>(_inputQueues.Select(q => q.Peek()));
        }

        private void Deplete()
        {
            _inputQueues.First(q => q.Count > 1).Dequeue();
        }
    }
}