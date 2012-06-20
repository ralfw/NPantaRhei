/*******************************************************************************
* Copyright 2009-2012 Amazon.com, Inc. or its affiliates. All Rights Reserved.
* 
* Licensed under the Apache License, Version 2.0 (the "License"). You may
* not use this file except in compliance with the License. A copy of the
* License is located at
* 
* http://aws.amazon.com/apache2.0/
* 
* or in the "license" file accompanying this file. This file is
* distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied. See the License for the specific
* language governing permissions and limitations under the License.
*******************************************************************************/

using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AmazonSQS_Sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            const string QUEUENAME = "MyQueue";

            AmazonSQS sqs = AWSClientFactory.CreateAmazonSQSClient();

            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("Getting Started with Amazon SQS");
                Console.WriteLine("===========================================\n");                
                
                //Creating a queue
                Console.WriteLine("Create a queue called MyQueue.\n");
                CreateQueueRequest sqsRequest = new CreateQueueRequest();
                sqsRequest.QueueName = QUEUENAME;
                CreateQueueResponse createQueueResponse = sqs.CreateQueue(sqsRequest);
                String myQueueUrl;
                myQueueUrl = createQueueResponse.CreateQueueResult.QueueUrl;

                //Confirming the queue exists
                ListQueuesRequest listQueuesRequest = new ListQueuesRequest();
                ListQueuesResponse listQueuesResponse = sqs.ListQueues(listQueuesRequest);


                var getQueueReq = new GetQueueUrlRequest();
                getQueueReq.QueueName = "AppZwitschern";
                var getQueueResp = sqs.GetQueueUrl(getQueueReq);
                Console.WriteLine(":: Url={0}", getQueueResp.GetQueueUrlResult.QueueUrl);


                Console.WriteLine("Printing list of Amazon SQS queues.\n");
                if (listQueuesResponse.IsSetListQueuesResult())
                {
                    ListQueuesResult listQueuesResult = listQueuesResponse.ListQueuesResult;
                    foreach (String queueUrl in listQueuesResult.QueueUrl)
                    {
                        Console.WriteLine("  QueueUrl: {0}", queueUrl);
                    }
                }
                Console.WriteLine();

                //Sending a message
                Console.WriteLine("Sending a message to MyQueue.\n");
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.QueueUrl = myQueueUrl; //URL from initial queue creation
                sendMessageRequest.MessageBody = "This is my message text.";
                sqs.SendMessage(sendMessageRequest);
                
                //Receiving a message
                ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
                receiveMessageRequest.QueueUrl = myQueueUrl;
                ReceiveMessageResponse receiveMessageResponse = sqs.ReceiveMessage(receiveMessageRequest);
                if (receiveMessageResponse.IsSetReceiveMessageResult())
                {
                    Console.WriteLine("Printing received message.\n");
                    ReceiveMessageResult receiveMessageResult = receiveMessageResponse.ReceiveMessageResult;
                    foreach (Message message in receiveMessageResult.Message)
                    {
                        Console.WriteLine("  Message");
                        if (message.IsSetMessageId())
                        {
                            Console.WriteLine("    MessageId: {0}", message.MessageId);
                        }
                        if (message.IsSetReceiptHandle())
                        {
                            Console.WriteLine("    ReceiptHandle: {0}", message.ReceiptHandle);
                        }
                        if (message.IsSetMD5OfBody())
                        {
                            Console.WriteLine("    MD5OfBody: {0}", message.MD5OfBody);
                        }
                        if (message.IsSetBody())
                        {
                            Console.WriteLine("    Body: {0}", message.Body);
                        }
                        foreach (Amazon.SQS.Model.Attribute attribute in message.Attribute)
                        {
                            Console.WriteLine("  Attribute");
                            if (attribute.IsSetName())
                            {
                                Console.WriteLine("    Name: {0}", attribute.Name);
                            }
                            if (attribute.IsSetValue())
                            {
                                Console.WriteLine("    Value: {0}", attribute.Value);
                            }
                        }
                    }
                }
                String messageRecieptHandle = receiveMessageResponse.ReceiveMessageResult.Message[0].ReceiptHandle;

                //Deleting a message
                Console.WriteLine("Deleting the message.\n");
                DeleteMessageRequest deleteRequest = new DeleteMessageRequest();
                deleteRequest.QueueUrl = myQueueUrl;
                deleteRequest.ReceiptHandle = messageRecieptHandle;
                sqs.DeleteMessage(deleteRequest);

            }
            catch (AmazonSQSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
            }

            Console.WriteLine("Press Enter to continue...");
            Console.Read();
        }
    }
}