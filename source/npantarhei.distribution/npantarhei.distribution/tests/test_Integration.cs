using System;
using NUnit.Framework;
using npantarhei.distribution.communication.local;
using npantarhei.runtime;
using npantarhei.runtime.contract;
using npantarhei.runtime.operations;

namespace npantarhei.distribution.tests
{
    [TestFixture]
    public class test_Integration
    {
        [Test]
        public void Run()
        {
            var bus = new SyncBus();
            var localTransceiver = bus.RegisterStandIn("local", "remote");
            var remoteTransceiver = bus.RegisterHost("remote");

            var configLocal = new FlowRuntimeConfiguration()
                                    .AddOperation(new StandInOperation("standin", localTransceiver, localTransceiver))
                                    .AddStream(".fin", "standin#fhelloworld")
                                    .AddStream("standin#fhelloworld", ".out")

                                    .AddStream(".ebcin", "standin#ebchelloworld.greet")
                                    .AddStream("standin#ebchelloworld.greeting", ".out");

            var configRemote = new FlowRuntimeConfiguration()
                                    .AddFunc<string, string>("fhelloworld", Greet)
                                    .AddStream(".@fhelloworld", "fhelloworld")
                                    .AddStream("fhelloworld", ".@fhelloworld")

                                    .AddEventBasedComponent("ebchelloworld", new Greeter())
                                    .AddStream(".greet@ebchelloworld", "ebchelloworld.greet")
                                    .AddStream("ebchelloworld.greeting", ".greeting@ebchelloworld");

            using(var localRuntime = new FlowRuntime(configLocal, new Schedule_for_sync_depthfirst_processing()))
            using(var remoteRuntime = new FlowRuntime(configRemote, new Schedule_for_sync_depthfirst_processing()))
            using(var host = new OperationHost(remoteRuntime, remoteTransceiver, remoteTransceiver))
            {
                localRuntime.Message += _ => Console.WriteLine("local: {0}", _);
                remoteRuntime.Message += _ => Console.WriteLine("    remote: {0}", _);

                IMessage result = null;
                localRuntime.Result += _ => result = _;

                localRuntime.Process(".fin", "peter");
                Assert.AreEqual("hello, peter", result.Data.ToString());

                localRuntime.Process(".ebcin", "mary");
                Assert.AreEqual("hello, mary", result.Data.ToString());
            }
        }


        string Greet(string name)
        {
            return "hello, " + name;
        }

        class Greeter
        {
            public void Greet(string name)
            {
                Greeting("hello, " + name);
            }

            public event Action<string> Greeting;
        }
    }
}
