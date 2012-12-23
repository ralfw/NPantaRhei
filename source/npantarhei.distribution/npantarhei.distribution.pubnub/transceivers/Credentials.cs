using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace npantarhei.distribution.pubnub.transceivers
{
    public class Credentials
    {
        public static Credentials Demo { get { return new Credentials("demo", "demo");}}

        public static Credentials LoadFrom(string filename)
        {
            using(var sr = new StreamReader(filename))
            {
                return new Credentials(sr.ReadLine(), 
                                       sr.ReadLine(), 
                                       sr.ReadLine());
            }
        }


        public Credentials(string publishingKey, string subscriptionKey) : this(publishingKey, subscriptionKey, ""){}
        public Credentials(string publishingKey, string subscriptionKey, string secretKey)
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
