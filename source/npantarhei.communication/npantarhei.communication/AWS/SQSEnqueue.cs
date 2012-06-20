using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using npantarhei.runtime.patterns;
using npantarhei.runtime.contract;

namespace npantarhei.communication.AWS
{
    public class SQSEnqueue : SQSBase
    {
        public SQSEnqueue(string queueName, AWSCredentials credentials) : this("SQSEnqueue", queueName, credentials) {}
        public SQSEnqueue(string name, string queueName, AWSCredentials credentials) : base(name, queueName, credentials) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (_queueUrl == null) Create_queue();
            Enqueue_message((string)input.Data);
        }

        private void Enqueue_message(string data)
        {
            var sendMessageRequest = new SendMessageRequest {
                QueueUrl = _queueUrl, 
                MessageBody = data 
            };
            _sqs.SendMessage(sendMessageRequest);
        }
    }
}
