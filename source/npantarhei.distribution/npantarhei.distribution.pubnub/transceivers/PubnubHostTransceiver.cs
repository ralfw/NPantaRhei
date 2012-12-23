using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using npantarhei.distribution.contract;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.pubnub.api;
using npantarhei.distribution.translators;

namespace npantarhei.distribution.pubnub.transceivers
{
    public class PubnubHostTransceiver : IHostStub, IStandInProxy
    {
        private readonly Pubnub _host;
        private readonly string _channel;
        private readonly Pubnub _standIn;


        public PubnubHostTransceiver(Credentials credentials, string channel)
        {
            _host = new Pubnub(credentials.PublishingKey, credentials.SubscriptionKey, credentials.SecretKey);
            _host.subscribe(channel, Process_input_from_standIn);
            _channel = channel;
            _standIn = new Pubnub(credentials.PublishingKey, credentials.SubscriptionKey, credentials.SecretKey);
        }


        #region IHostStub
        public event Action<HostInput> ReceivedFromStandIn;

        public string HostEndpointAddress
        {
            get { return _channel; }
        }
        #endregion

        #region IStandInProxy
        public void SendToStandIn(Tuple<string, HostOutput> output)
        {
            var outputSerialized = output.Item2.Serialize();
            _standIn.publish(output.Item1, outputSerialized, _ => { });
        }
        #endregion


        void Process_input_from_standIn(object pubnubMsg)
        {
            var serializedInput = (string)((JValue)((ReadOnlyCollection<object>)pubnubMsg)[0]).Value;
            var standInInput = (HostInput)Convert.FromBase64String(serializedInput).Deserialize();
            ReceivedFromStandIn(standInInput);
        }

        public void Dispose()
        {
            _host.unsubscribe(_channel, _ => {});
        }
    }
}
