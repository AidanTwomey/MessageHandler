using System;
using Xunit;
using MessageHandler;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using NSubstitute;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MessageHandlerTests
{
    public class EventHandlerTests
    {
        [Fact]
        public async void ReadsXmlFromBucket()
        {
            string handledMessage = await GetHandledMessage();
            Message message = new Message(handledMessage);

            Assert.Equal( "Pink Floyd", message.Name);
        }

        private async Task<string> GetHandledMessage()
        {
            IAmazonS3 client = Substitute.For<IAmazonS3>();
            ILambdaContext context = Substitute.For<ILambdaContext>();
            Stream responseStream = Substitute.For<Stream>();
            TextReader reader = Substitute.For<TextReader>();

            GetObjectResponse response = new GetObjectResponse(){ ResponseStream = responseStream};


            var filePath = Path.Combine( "TestData", "aDex.xml");

            reader.ReadToEnd().Returns(XDocument.Load(filePath).ToString());

            client.GetObjectAsync( Arg.Any<string>(), Arg.Any<string>() ).Returns( response );

            S3Event s3event = new S3Event(){
                Records = new List<S3EventNotification.S3EventNotificationRecord>(){ 
                  new S3EventNotification.S3EventNotificationRecord(){
                    S3 = new S3EventNotification.S3Entity(){
                        Bucket = new S3EventNotification.S3BucketEntity(){Name="sourcebucket"},
                        Object =  new S3EventNotification.S3ObjectEntity(){Key = "aDex.xml"} } }
              }
            };

            var handler = new DefinitionUploadedEventHandler(client, reader);
            var messageTask = Task.Run( () => ( handler.HandleUploadEvent( s3event, context)) );

            return await messageTask;
        }
    }
}