using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using System.IO;
using Amazon.S3.Model;

namespace MessageHandler
{
    public class DefinitionUploadedEventHandler
    {
        private readonly IAmazonS3 s3client;
        private readonly Func<Stream, TextReader> createStreamReader;
        public DefinitionUploadedEventHandler()
        {
            this.s3client = new AmazonS3Client();
            this.createStreamReader = s => new StreamReader(s);
        }

        public DefinitionUploadedEventHandler(IAmazonS3 s3Client, TextReader streamReader)
        {
            this.s3client = s3Client;
            this.createStreamReader = s => streamReader;
        }

        [LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<string> HandleUploadEvent(S3Event evnt, ILambdaContext cont)
        {
            var logger = cont.Logger;
            var s3Event = evnt.Records[0].S3;

            logger.LogLine( String.Format( "bucket {0}, key {1}", s3Event.Bucket.Name, s3Event.Object.Key) );

            var request = new GetObjectRequest(){
                BucketName = s3Event.Bucket.Name,
                Key = s3Event.Object.Key,
                ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.None
            };

            try
            {
              logger.LogLine( "Waiting for response" );
              var response = await s3client.GetObjectAsync(request);

              using (var stream = response.ResponseStream)
              {
                TextReader tr = createStreamReader(stream);
                var s3Document = tr.ReadToEnd();
                logger.LogLine("Handler: Got Data from Key");
                                
                var message = new MessageParser().Parse(s3Document);

                // write this to Dynomo DB

                return message.Name;
              }
            }
            catch ( Exception ex)
            {
                logger.LogLine( ex.ToString() + ex.StackTrace );

                return ex.Message;
            }
        }
    }
}