using System;
using NUnit.Framework;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.wcf.transceivers;

namespace npantarhei.distribution.wcf.tests
{
    [TestFixture]
    public class test_Transceivers
    {
        [Test]
        public void StandIn_to_Host()
        {
            using(var host = new WcfHostTransceiver("localhost:8000"))
            using (var standIn = new WcfStandInTransceiver("localhost:8001", "localhost:8000"))
            {
                HostInput input = null;
                host.ReceivedFromStandIn += _ => input = _;
 
                standIn.SendToHost(new HostInput {Data = new byte[]{4,2}});

                Assert.AreEqual(new byte[]{4,2}, input.Data);
            }
        }


        [Test]
        public void Host_to_StandIn()
        {
            using (var host = new WcfHostTransceiver("localhost:8000"))
            using (var standIn = new WcfStandInTransceiver("localhost:8001", "localhost:8000"))
            {
                HostOutput output = null;
                standIn.ReceivedFromHost += _ => output = _;

                host.SendToStandIn(new Tuple<string, HostOutput>("localhost:8001", new HostOutput{Data = new byte[]{4,2}}));

                Assert.AreEqual(new byte[]{4,2}, output.Data);
            }
        }
    }
}
