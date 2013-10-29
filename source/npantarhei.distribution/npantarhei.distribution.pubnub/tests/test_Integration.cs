using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.distribution.contract.messagetypes;
using npantarhei.distribution.pubnub.api;
using npantarhei.distribution.pubnub.transceivers;
using npantarhei.distribution.translators;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;
using npantarhei.runtime.operations;

namespace npantarhei.distribution.pubnub.tests
{
    [TestFixture]
    public class test_Integration
    {
        [Test]
        public void Run()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");

            var configServer = new FlowRuntimeConfiguration()
                                    .AddFunc<string, string>("hello", s => "hello, " + s)
                                    .AddStream(".@hello", "hello")
                                    .AddStream("hello", ".@hello");
            using(var server = new FlowRuntime(configServer))
            using (new PubnubOperationHost(server, cre, "host"))
            {
                server.Message += Console.WriteLine;

                var configClient = new FlowRuntimeConfiguration()
                                    .AddOperation(new PubnubStandInOperation("standin", cre, "host"))
                                    .AddStream(".in", "standin#hello")
                                    .AddStream("standin#hello", ".out");
                using(var client = new FlowRuntime(configClient))
                {
                    client.Message += Console.WriteLine;

                    client.Process(".in", "peter");

                    var result = "";
                    Assert.IsTrue(client.WaitForResult(5000, _ => result = (string)_.Data));

                    Assert.AreEqual("hello, peter", result);
                }
            }
        }


        [Test]
        public void Connect_transceivers()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");

            using(var host = new PubnubHostTransceiver(cre, "thehost"))
            {
                var config = new FlowRuntimeConfiguration()
                                .AddOperation(new PubnubStandInOperation("op", cre, "thehost"))
                                .AddStream(".in", "op#greeting")
                                .AddStream("op#greeting", ".out");
                using (var fr = new FlowRuntime(config))
                {
                    fr.Message += Console.WriteLine;

                    host.ReceivedFromStandIn += rhi =>
                                                    {
                                                        var data = (string)rhi.Data.Deserialize();
                                                        var ho = new HostOutput
                                                                    {
                                                                        CorrelationId=rhi.CorrelationId,
                                                                        Data =(data + "y").Serialize(),
                                                                        Portname = "greeting"
                                                                    };

                                                        ThreadPool.QueueUserWorkItem(_ =>
                                                        {
                                                            host.SendToStandIn(new Tuple<string, HostOutput>(rhi.StandInEndpointAddress, ho));
                                                        });
                                                    };

                    fr.Process(".in", "hello");

                    var result = "";
                    Assert.IsTrue(fr.WaitForResult(5000, _ => result = _.Data as string));
                    Assert.AreEqual("helloy", result);
                }
            }    
        }


        [Test]
        public void Connect_transceivers_operationhost()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");

            var configServer = new FlowRuntimeConfiguration()
                        .AddFunc<string, string>("greeting", s => s + "x")
                        .AddStream(".@greeting", "greeting")
                        .AddStream("greeting", ".@greeting");
            using (var server = new FlowRuntime(configServer))
            using (var serverhost = new PubnubOperationHost(server, cre, "thehost"))
            {
                server.Message += Console.WriteLine;

                var config = new FlowRuntimeConfiguration()
                                .AddOperation(new PubnubStandInOperation("op", cre, "thehost"))
                                .AddStream(".in", "op#greeting")
                                .AddStream("op#greeting", ".out");
                using (var fr = new FlowRuntime(config))
                {
                    fr.Message += Console.WriteLine;

                    fr.Process(".in", "hello");

                    var result = "";
                    Assert.IsTrue(fr.WaitForResult(5000, _ => result = _.Data as string));
                    Assert.AreEqual("hellox", result);
                }
            }
        }


        [Test]
        public void Test()
        {
            var pnclient = new Pubnub("demo", "demo");
            var pnserver = new Pubnub("demo", "demo");

            var are = new AutoResetEvent(false);
            Console.WriteLine("before subscribe");
            ThreadPool.QueueUserWorkItem(___ =>
                                             {
                                                 pnclient.subscribe("client", (object msg) =>
                                                                                  {
                                                                                      Console.WriteLine("received sth");
                                                                                      are.Set();
                                                                                  },
                                                                    (object msg2) =>
                                                                        {
                                                                            Console.WriteLine("subscribed");
                                                                        });
                                             });
            Console.WriteLine("before sleep");
            Thread.Sleep(2000);
            Console.WriteLine("after sleep");

            ThreadPool.QueueUserWorkItem(__ =>
                                             {
                                                 pnserver.publish("client", "hello",
                                                                  _ => { Console.WriteLine("sent"); });
                                             });
            Console.WriteLine("after publish");

            Assert.IsTrue(are.WaitOne(5000));
        }


        [Test]
        public void Test_request_response()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");

            var are = new AutoResetEvent(false);

            var pnclient = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
            pnclient.subscribe("client", (object _) =>
                                                {
                                                    Console.WriteLine("client received response");
                                                    are.Set();
                                                },
                                            (object _) =>
                                                {
                                                    Console.WriteLine("subscribed client");
                                                });


            var pnserver = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
            pnserver.subscribe("server", (object _) =>
                                                {
                                                    Console.WriteLine("server received request");
                                                    pnserver.publish("client", "myresponse", __ => Console.WriteLine("responded from server"));
                                                },
                                            (object _) => Console.WriteLine("subscribed server"));

            Thread.Sleep(1000); // test fails with a delay here

            var pnclient2 = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
            pnclient2.publish("server", "myrequest", _ => Console.WriteLine("sent from client"));


            Assert.IsTrue(are.WaitOne(10000));
        }


        [Test]
        public void Test_request_response2()
        {
            var cre = PubnubCredentials.LoadFrom("pubnub credentials.txt");

            var are = new AutoResetEvent(false);

            var pn = new Pubnub(cre.PublishingKey, cre.SubscriptionKey, cre.SecretKey);
            pn.subscribe("client", (object _) =>
                                        {
                                            Console.WriteLine("client received response");
                                            are.Set();
                                        },
                                            (object _) =>
                                            {
                                                Console.WriteLine("subscribed client");
                                            });


            pn.subscribe("server", (object _) =>
            {
                Console.WriteLine("server received request");
                pn.publish("client", "myresponse", __ => Console.WriteLine("responded from server"));
            },
                                            (object _) => Console.WriteLine("subscribed server"));

            Thread.Sleep(1000); // test fails with a delay here

            pn.publish("server", "myrequest", _ => Console.WriteLine("sent from client"));


            Assert.IsTrue(are.WaitOne(10000));
        }
    }
}
