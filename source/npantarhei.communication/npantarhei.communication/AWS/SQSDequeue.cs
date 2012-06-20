using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.SQS.Model;
using npantarhei.runtime.contract;
using npantarhei.runtime.messagetypes;

namespace npantarhei.communication.AWS
{
    public class SQSDequeue : SQSBase
    {
        public SQSDequeue(string queueName, AWSCredentials credentials) : this("SQSDequeue", queueName, credentials) {}
        public SQSDequeue(string name, string queueName, AWSCredentials credentials) : base(name, queueName, credentials) {}

        protected override void Process(IMessage input, Action<IMessage> continueWith, Action<FlowRuntimeException> unhandledException)
        {
            if (_queueUrl == null) Create_queue();
            Dequeue_message(continueWith);
        }

        private void Dequeue_message(Action<IMessage> continueWith)
        {
            var n_messages_received = 0;
            do
            {
                n_messages_received = 0;

                var receiveMessageResponse = _sqs.ReceiveMessage(new ReceiveMessageRequest {QueueUrl = _queueUrl, MaxNumberOfMessages = 10});
                var receiveMessageResult = receiveMessageResponse.ReceiveMessageResult;
                foreach (var message in receiveMessageResult.Message)
                {
                    continueWith(new runtime.messagetypes.Message(base.Name, message.Body));
                    Delete_message(message.ReceiptHandle);
                    n_messages_received++;
                }
            } while (n_messages_received > 0);
        }

        private void Delete_message(string receiptHandle)
        {
            var deleteRequest = new DeleteMessageRequest {
                QueueUrl = _queueUrl, 
                ReceiptHandle = receiptHandle
            };
            _sqs.DeleteMessage(deleteRequest);
        }
    }
}
