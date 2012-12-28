using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace npantarhei.distribution.pubnub.transceivers
{
    public class PubnubCredentials
    {
        public static PubnubCredentials Demo { get { return new PubnubCredentials("demo", "demo");}}

        public static PubnubCredentials LoadFrom(string filename)
        {
            using(var sr = new StreamReader(filename))
            {
                return new PubnubCredentials(sr.ReadLine(), 
                                       sr.ReadLine(), 
                                       sr.ReadLine());
            }
        }


        public PubnubCredentials(string publishingKey, string subscriptionKey) : this(publishingKey, subscriptionKey, ""){}
        public PubnubCredentials(string publishingKey, string subscriptionKey, string secretKey)
        {
            PublishingKey = publishingKey;
            SubscriptionKey = subscriptionKey;
            SecretKey = secretKey;
        }

        public string PublishingKey { get; private set; }
        public string SubscriptionKey { get; private set; }
        public string SecretKey { get; set; }
    }
}
