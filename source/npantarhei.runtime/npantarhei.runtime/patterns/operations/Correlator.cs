using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.runtime.patterns.operations
{
    public class Correlation
    {
        public Correlation(object data) : this(Guid.NewGuid(), data) { }
        public Correlation(Guid id, object data)
        {
            Id = id;
            Data = data;
        }

        public Guid Id { get; private set; }
        public object Data { get; private set; }
    }

    /* Map a message carrying a Correlation to a message with the corrId and the data - and vice versa.
     * a -Correlation-> encode.Correlator.encoded -> ...
     * ... -> decode.Correlator.decoded -Correlation-> b
     */
    public class Correlator : AOperation
    {
        public Correlator() : this("correlator") { }
        public Correlator(string name) : base(name) { }

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if ("encode decode".IndexOf(input.Port.Name.ToLower()) < 0) throw new ArgumentException("Invalid input port! Valid input ports of Correlator are .encode and .decode.");

            IMessage msg = null;
            Correlation corr = null;
            switch (input.Port.Name.ToLower())
            {
                case "encode":
                    corr = (Correlation)input.Data;
                    msg = new Message(base.Name + ".encoded", corr.Data, corr.Id);
                    break;

                case "decode":
                    corr = new Correlation(input.CorrelationId, input.Data);
                    msg = new Message(base.Name + ".decoded", corr, corr.Id);
                    break;
            }

            msg.Causalities = input.Causalities;
            msg.FlowStack = input.FlowStack;
            continueWith(msg);
        }
    }
}
