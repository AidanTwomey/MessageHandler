using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace MessageHandler
{

    public class Message
    {
        public Message(string name, string source)
        {
            Name = name;
            Source = source;
        }

        [JsonProperty("name")]
        public string Name { get; private set;}

        [JsonProperty("source")]
        public string Source { get; private set;}
    }
}