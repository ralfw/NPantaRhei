using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns
{
    class PushCausality : IOperation
    {
        private readonly string _name;
        private readonly OperationAdapter _implementation;


        public PushCausality(string name, IPort exceptionHandler)
        {
            _name = name;
            _implementation = (input, outputCont, _) =>
                                  {
                                      input.Causalities.Push(exceptionHandler);
                                      outputCont(input);
                                  };
        }

        public string Name
        {
            get { return _name; }
        }

        public OperationAdapter Implementation
        {
            get { return _implementation; }
        }
    }
}
