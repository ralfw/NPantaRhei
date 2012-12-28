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
    public class PubnubStandInTransceiver : IHostProxy, IStandInStub
    {
        private readonly Pubnub _transceiver;
        private readonly string _receiverChannel;
        private readonly string _hostChannel;


        public PubnubStandInTransceiver(PubnubCredentials credentials, string hostChannel)
        {
            _transceiver = new Pubnub(credentials.PublishingKey, credentials.SubscriptionKey, credentials.SecretKey);
            _receiverChannel = Guid.NewGuid().ToString();
            _transceiver.subscribe(_receiverChannel, Process_output_from_host);

            _hostChannel = hostChannel;
        }


        #region IHostProxy
        public void SendToHost(HostInput input)
        {
            var serializedInput = input.Serialize();
            _transceiver.publish(_hostChannel, serializedInput, _ => { });
        }
        #endregion


        #region IStandInStub
        public event Action<HostOutput> ReceivedFromHost;

        public string StandInEndpointAddress
        {
            get { return _receiverChannel; }
        }
        #endregion


        void Process_output_from_host(object pubnubMsg)
        {
            var serializedOutput = (string)((JValue)((ReadOnlyCollection<object>)pubnubMsg)[0]).Value;
            var hostOutput = (HostOutput)Convert.FromBase64String(serializedOutput).Deserialize();
            ReceivedFromHost(hostOutput);
        }


        public void Dispose()
        {
            _transceiver.unsubscribe(_receiverChannel, _ => { });
        }
    }
}
