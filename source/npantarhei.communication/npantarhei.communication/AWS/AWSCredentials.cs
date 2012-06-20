namespace npantarhei.communication.AWS
{
    public class AWSCredentials
    {
        private readonly string _awsKey;
        private readonly string _awsSecret;

        public AWSCredentials(string awsKey, string awsSecret)
        {
            _awsKey = awsKey;
            _awsSecret = awsSecret;
        }

        public string AwsKey
        {
            get { return _awsKey; }
        }

        public string AwsSecret
        {
            get { return _awsSecret; }
        }
    }
}