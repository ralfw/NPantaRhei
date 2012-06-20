using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using npantarhei.runtime.patterns;

namespace npantarhei.communication.AWS
{
    public class SQSBase : AOperation
    {
        protected readonly AmazonSQS _sqs;
        private readonly string _queueName;
        protected string _queueUrl;

        protected SQSBase(string name, string queueName, AWSCredentials awsCredentials) : base(name)
        {
            _queueName = queueName;
            _sqs = AWSClientFactory.CreateAmazonSQSClient(awsCredentials.AwsKey, awsCredentials.AwsSecret);
        }

        protected void Create_queue()
        {
            var createQueueResponse = _sqs.CreateQueue(new CreateQueueRequest {QueueName = _queueName});
            _queueUrl = createQueueResponse.CreateQueueResult.QueueUrl;
        }
    }
}