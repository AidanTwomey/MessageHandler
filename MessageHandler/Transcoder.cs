using Amazon.ElasticTranscoder;
using Amazon;

namespace MessageHandler
{
    public class Transcoder
    {
        private readonly AmazonElasticTranscoderClient transcoder;

        public Transcoder()
        {
            transcoder = new AmazonElasticTranscoderClient(new AmazonElasticTranscoderConfig(){RegionEndpoint = RegionEndpoint.EUWest1 } );
        }

        public void Transcode()
        {
            //transcoder.CreateJobAsync()
        }
    }
}