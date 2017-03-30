using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.Lambda.SNSEvents;
using System.Linq;

namespace MessageHandler
{
    public class TranscodeRequestEventHandler
    {
        public readonly Transcoder _transcoder;

        public TranscodeRequestEventHandler() 
        {
            _transcoder = new Transcoder();
        }
        
        [LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<string> Transcode(SNSEvent s3event, ILambdaContext context)
        {
            var sourceFile = s3event.Records.First().Sns.Message;
                             
            context.Logger.LogLine( sourceFile );
            
            return await _transcoder.Transcode(sourceFile, context.Logger);
        }
    }
}