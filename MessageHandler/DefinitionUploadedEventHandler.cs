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
        public async Task<string> HandleUploadEvent(S3Event s3event, ILambdaContext context)
        {
            var logger = context.Logger;
            var s3Record = s3event.Records[0].S3;

            logger.LogLine( String.Format( "bucket {0}, key {1}", s3Record.Bucket.Name, s3Record.Object.Key) );

            var request = new GetObjectRequest(){
                BucketName = s3Record.Bucket.Name,
                Key = s3Record.Object.Key,
                ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.None
            };

            var response = await s3client.GetObjectAsync(request);

            var message = ParseResponse(response, logger);

            // write this to dynamo db

            return message.Name;
                        
        }

        private Message ParseResponse(GetObjectResponse response, ILambdaLogger logger)
        {
            using (var stream = response.ResponseStream)
            {
                TextReader tr = createStreamReader(stream);
                var s3Document = tr.ReadToEnd();
                logger.LogLine("Handler: Got Data from Key");
                                
                return new MessageParser().Parse(s3Document);
            }
        }
    }
}