using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using System.IO;

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
        public async Task<string> HandleUploadEvent(S3Event evnt, ILambdaContext cont)
        {
            var s3Event = evnt.Records[0].S3;

            Console.WriteLine( "bucket {0}, key {1}", s3Event.Bucket.Name, s3Event.Object.Key );

            var response = await this.s3client.GetObjectAsync(s3Event.Bucket.Name, s3Event.Object.Key);
        
            using (var stream = response.ResponseStream)
            {
                TextReader tr = createStreamReader(stream);
                var s3Document = tr.ReadToEnd();
                Console.WriteLine("Handler: Got Data from Key");
                                
                var message = new MessageParser().Parse(s3Document);

                // write this to Dynomo DB

                return message.Name;
            }
        }
    }
}