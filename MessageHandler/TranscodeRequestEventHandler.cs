using System;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using Amazon.Lambda.SNSEvents;
using Amazon;
using Amazon.ElasticTranscoder.Model;
using System.Linq;
// using Amazon.S3;
// using System.IO;
// using Amazon.S3.Model;

namespace MessageHandler
{
    public class TranscodeRequestEventHandler
    {
        
        [LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<string> Transcode(SNSEvent s3event, ILambdaContext context)
        {
            var sourceFile = s3event.Records.First().Sns.Message;

            context.Logger.LogLine( sourceFile );

            var transcoder = new Amazon.ElasticTranscoder.AmazonElasticTranscoderClient(RegionEndpoint.EUWest1);
            
            var pipelines = await transcoder.ListPipelinesAsync();
            var pipeline = pipelines.Pipelines.First( p => p.Name == "adex_pipeline");

            pipeline.Role = "arn:aws:iam::160534783289:role/Elastic_Transcoder_Default_Role";
            pipeline.InputBucket = "myflacs";
            pipeline.OutputBucket = "mydeflacedfiles";

            context.Logger.LogLine( String.Format("Pipeline Created: {0}, {1}", pipeline.Id, pipeline.Name) );

            var response = await transcoder.CreateJobAsync( new CreateJobRequest(){
                PipelineId = pipeline.Id,
                
                Input = new JobInput()
                {
                    Container = "auto",
                    Key = s3event.Records.First().Sns.Message
                },
                Output = new CreateJobOutput()
                {
                    Key = String.Format( "{0}.mp3", sourceFile),
                    PresetId = "1351620000001-300020"
                }
            } );

            context.Logger.LogLine( response.HttpStatusCode.ToString() );

            return "transcoded " + sourceFile;
        }
    }
}