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
        public PushCausality(string name) : base(name) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            input.Causalities.Push(new Port(input.Port.Fullname + ".exception"));
            continueWith(input);   
        }
    }
}
