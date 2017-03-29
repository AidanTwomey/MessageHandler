using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using System.IO;
using Amazon.S3.Model;
using Amazon;

namespace MessageHandler
{
    public class DefinitionUploadedEventHandler
    {
        private readonly IAmazonS3 s3client;
        private readonly Func<Stream, TextReader> createStreamReader;
        private readonly IPersister persister;

        public DefinitionUploadedEventHandler()
        {
            this.s3client = new AmazonS3Client();
            this.createStreamReader = s => new StreamReader(s);
            this.persister = new DynamoNamePersister(new Amazon.DynamoDBv2.AmazonDynamoDBClient());
        }

        public DefinitionUploadedEventHandler(IAmazonS3 s3Client, TextReader streamReader, IPersister persister)
        {
            this.s3client = s3Client;
            this.createStreamReader = s => streamReader;
            this.persister = persister;
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

            var document = persister.Persist(message, logger);

            NotifyAsync( message.Source );

            return document.ToString();
                        
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

        private async void NotifyAsync(string message)
        {
            var client = new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(RegionEndpoint.EUWest1);

            await client.PublishAsync( "arn:aws:sns:eu-west-1:160534783289:adex_parsed", message);
        }
    }
}