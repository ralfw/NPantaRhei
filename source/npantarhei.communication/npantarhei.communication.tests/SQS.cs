using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.communication.tests
{
    [TestFixture]
    public class SQS
    {
        private const string QUEUE_NAME = "npantarhei-communication-tests";

        [Test, Explicit]
        public void Enqueue_and_dequeue()
        {
            var credentials = new AWS.AWSCredentials("AKIAJASAOE6E6U4NEFLA", "LOUDf904YumcvB35KZ3wVJtKXrvEGQYSk4JrS4PE");
            var enqueue = new AWS.SQSEnqueue(QUEUE_NAME, credentials);
            var dequeue = new AWS.SQSDequeue(QUEUE_NAME, credentials);

            enqueue.Implementation(new Message("x", "hello " + DateTime.Now), null, null);
            enqueue.Implementation(new Message("x", "world " + DateTime.Now), null, null);
            for(var i=0; i<23; i++)
                enqueue.Implementation(new Message("x", "world " + i + " " + DateTime.Now), null, null);

            dequeue.Implementation(new Message("x", null), _ => Console.WriteLine(_.Data), null);
        }
    }
}
