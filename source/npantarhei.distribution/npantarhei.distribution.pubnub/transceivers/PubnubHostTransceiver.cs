using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using npantarhei.distribution.contract;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.pubnub.api;
using npantarhei.distribution.translators;

namespace npantarhei.distribution.pubnub.transceivers
{
    public class PubnubHostTransceiver : IHostStub, IStandInProxy
    {
        private readonly Pubnub _transceiver;
        private readonly string _channel;


        public PubnubHostTransceiver(PubnubCredentials credentials, string channel)
        {
            _transceiver = new Pubnub(credentials.PublishingKey, credentials.SubscriptionKey, credentials.SecretKey);
            _transceiver.subscribe(channel, Process_input_from_standIn);
            _channel = channel;
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
            Console.WriteLine("sent to standin @ {0}", Thread.CurrentThread.GetHashCode());
            var outputSerialized = output.Item2.Serialize();
            _transceiver.publish(output.Item1, outputSerialized, _ =>
                                                                 {
                                                                     Console.WriteLine("sent");
                                                                 });
        }
        #endregion


        void Process_input_from_standIn(object pubnubMsg)
        {
            Console.WriteLine("receive from standin @ {0}", Thread.CurrentThread.GetHashCode());
            var serializedInput = (string)((JValue)((ReadOnlyCollection<object>)pubnubMsg)[0]).Value;
            var standInInput = (HostInput)Convert.FromBase64String(serializedInput).Deserialize();
            ReceivedFromStandIn(standInInput);
        }


        public void Dispose()
        {
            _transceiver.unsubscribe(_channel, _ => {});
        }
    }
}
