using System;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.distribution.translators
{
    class HostTranslator
    {
        private readonly CorrelationCache<string> _cache = new CorrelationCache<string>();
 

        public void Process_remote_input(HostInput input)
        {
            _cache.Add(input.CorrelationId, input.StandInEndpointAddress);
            var msg = new Message(input.Portname.StandInPortnameToInputPortname(), input.Data.Deserialize(), input.CorrelationId);
            Translated_input(msg);
        }

        public event Action<IMessage> Translated_input;


        public void Process_local_output(IMessage message)
        {
            var standInEndpointAddress = _cache.Get(message.CorrelationId);
            var output = new HostOutput { Portname = message.Port.OutputPortToStandInPortname(), Data = message.Data.Serialize(), CorrelationId = message.CorrelationId};
            Translated_output(new Tuple<string, HostOutput>(standInEndpointAddress, output));
        }

        public event Action<Tuple<string, HostOutput>> Translated_output;
    }
}
