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
    public class test_PubnubStandInTransceiver
    {
        [Test]
        public void Send_to_host()
        {
            var cre = Credentials.LoadFrom("pubnub credentials.txt");

            var host = new Pubnub(cre.PublishingKey, cre.SubscriptionKey);
            try
            {
                var are = new AutoResetEvent(false);
                ReadOnlyCollection<object> result = null;
                host.subscribe("hostchannel", (ReadOnlyCollection<object> _) =>
                {
                    result = _;
                    are.Set();
                }); 
                
                using (var sut = new PubnubStandInTransceiver(cre, "hostchannel"))
                {
                    var hi = new HostInput
                                 {
                                     CorrelationId = Guid.NewGuid(),
                                     Data = "hello".Serialize(),
                                     Portname = "portname",
                                     StandInEndpointAddress = "endpoint"
                                 };
                    sut.SendToHost(hi);

                    Assert.IsTrue(are.WaitOne(5000));

                    var hiReceived = Convert.FromBase64String((string) ((JValue) result[0]).Value).Deserialize() as HostInput;
                    Assert.AreEqual(hi.CorrelationId, hiReceived.CorrelationId);
                    Assert.AreEqual(hi.Data, hiReceived.Data);
                    Assert.AreEqual(hi.Portname, hiReceived.Portname);
                    Assert.AreEqual(hi.StandInEndpointAddress, hiReceived.StandInEndpointAddress);
                }
            }
            finally
            {
                host.unsubscribe("hostchannel", _ => {});
            }
        }


        [Test]
        public void Receive_from_host()
        {
            var cre = Credentials.LoadFrom("pubnub credentials.txt");

            using (var sut = new PubnubStandInTransceiver(cre, "hostchannel"))
            {
                var are = new AutoResetEvent(false);
                HostOutput result = null;
                sut.ReceivedFromHost += _ =>
                                            {
                                                result = _;
                                                are.Set();
                                            };

                var sender = new Pubnub(cre.PublishingKey, cre.SubscriptionKey);
                var ho = new HostOutput {CorrelationId = Guid.NewGuid(), Data = "hello".Serialize(), Portname = "portname"};
                sender.publish(sut.StandInEndpointAddress, ho.Serialize(), _ => { });

                Assert.IsTrue(are.WaitOne(5000));

                Assert.AreEqual(ho.CorrelationId, result.CorrelationId);
                Assert.AreEqual(ho.Data, result.Data);
                Assert.AreEqual(ho.Portname, result.Portname);
            }
        }
    }
}
