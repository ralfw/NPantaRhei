using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.patterns.operations;

namespace npantarhei.runtime.patterns
{
    class PopCausality : AOperation
    {
        public PopCausality(string name) : base(name) {}

        protected override void Process(IMessage input, System.Action<IMessage> continueWith, System.Action<FlowRuntimeException> unhandledException)
        {
 	        input.Causalities.Pop();
            continueWith(input);
        }
    }
}