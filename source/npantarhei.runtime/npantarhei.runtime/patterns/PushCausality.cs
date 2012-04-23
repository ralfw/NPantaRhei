using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.patterns
{
    class PushCausality : AOperation
    {
        public PushCausality(string name, IPort exceptionHandler) : base(name)
        {
            base.Implementation = (input, outputCont, _) =>
                                        {
                                            input.Causalities.Push(exceptionHandler);
                                            outputCont(input);
                                        };
        }
    }
}
