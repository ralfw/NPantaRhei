using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using npantarhei.distribution.pubnub.api;

namespace npantarhei.distribution.pubnub.tests  
{
    [TestFixture]
    public class explore_PubNub
    {
        [Test, Explicit]
        public void Run()
        {
            var are = new AutoResetEvent(false);

            var pubnubServer = new Pubnub("demo", "demo");
            pubnubServer.subscribe("testchannel", (ReadOnlyCollection<object> _) =>
                                                      {
                                                          var v = _[0] as JValue;
                                                          var a = Convert.FromBase64String((string)v.Value);
                                                          Console.WriteLine("received: {0}", a);
                                                          are.Set();
                                                      });

            var pubnubClient = new Pubnub("demo", "demo");
            pubnubClient.publish("testchannel", new byte[]{1,2,3}, _ => Console.WriteLine("published"));

            Assert.IsTrue(are.WaitOne(5000));
        }
    }
}
