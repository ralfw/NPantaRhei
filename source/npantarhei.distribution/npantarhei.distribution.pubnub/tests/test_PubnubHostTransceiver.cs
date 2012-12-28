using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.pubnub.api;
using npantarhei.distribution.pubnub.transceivers;
using npantarhei.distribution.translators;

namespace npantarhei.distribution.pubnub.tests
{
    [TestFixture]
    public class test_PubnubHostTransceiver
    {
        [Test]
        public void Receive_from_standIn()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");
            using (var sut = new PubnubHostTransceiver(cre, "hostchannel"))
            {

                var are = new AutoResetEvent(false);
                HostInput result = null;
                sut.ReceivedFromStandIn += _ =>
                                               {
                                                   result = _;
                                                   are.Set();
                                               };

                var standIn = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
                var hi = new HostInput{CorrelationId = Guid.NewGuid(), Data = "hello".Serialize(), Portname = "portname", StandInEndpointAddress = "endpoint"};
                standIn.publish("hostchannel", hi.Serialize(), _ => { });

                Assert.IsTrue(are.WaitOne(5000));

                Assert.AreEqual(hi.CorrelationId, result.CorrelationId);
                Assert.AreEqual(hi.Data, result.Data);
                Assert.AreEqual(hi.Portname, result.Portname);
                Assert.AreEqual(hi.StandInEndpointAddress, result.StandInEndpointAddress);
            }
        }

        [Test]
        public void Send_to_standIn()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");
            using(var sut = new PubnubHostTransceiver(cre, "hostchannel"))
            {
                var standIn = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
                try
                {
                    var standInChannel = Guid.NewGuid().ToString();

                    var are = new AutoResetEvent(false);
                    ReadOnlyCollection<object> result = null;
                    standIn.subscribe(standInChannel, (ReadOnlyCollection<object> _) =>
                                              {
                                                  result = _;
                                                  are.Set();
                                              });

                    var ho = new HostOutput{CorrelationId = Guid.NewGuid(), Data = "hello".Serialize(), Portname = "portname"};
                    sut.SendToStandIn(new Tuple<string, HostOutput>(standInChannel, ho));

                    Assert.IsTrue(are.WaitOne(5000));

                    var hoReceived = Convert.FromBase64String((string)((JValue)result[0]).Value).Deserialize() as HostOutput;
                    Assert.AreEqual(ho.CorrelationId, hoReceived.CorrelationId);
                    Assert.AreEqual(ho.Data, hoReceived.Data);
                    Assert.AreEqual(ho.Portname, hoReceived.Portname);
                }
                finally
                {
                    standIn.subscribe("standIn", _ => {});
                }
            }
        }
    }
}
