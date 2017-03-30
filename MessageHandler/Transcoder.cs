using Amazon.ElasticTranscoder;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.ElasticTranscoder.Model;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace MessageHandler
{
    public class Transcoder
    {
        private readonly AmazonElasticTranscoderClient transcoder;

        private const string MP3_PRESET =  "1351620000001-300020";

        public Transcoder()
        {
            transcoder = new AmazonElasticTranscoderClient(new AmazonElasticTranscoderConfig(){RegionEndpoint = RegionEndpoint.EUWest1 } );
        }

        public async Task<string> Transcode(string sourceFile, ILambdaLogger logger)
        {
            var pipelines = await transcoder.ListPipelinesAsync();
            var pipeline = pipelines.Pipelines.First( p => p.Name == "adex_pipeline");

            logger.LogLine( String.Format("Found pipeline {0} with input {1}, output {2}", pipeline.Name, pipeline.InputBucket, pipeline.OutputBucket));

            pipeline.Role = "arn:aws:iam::160534783289:role/Elastic_Transcoder_Default_Role";
            
            var response = await transcoder.CreateJobAsync( new CreateJobRequest(){
                PipelineId = pipeline.Id,
                
                Input = new JobInput()
                {
                    Container = "auto",
                    Key = sourceFile
                },
                Output = new CreateJobOutput()
                {
                    Key = String.Format( "{0}.mp3", sourceFile),
                    PresetId = MP3_PRESET
                }
            } );

            logger.LogLine( response.HttpStatusCode.ToString() );

            return "transcoded " + sourceFile;
        }
    }
}