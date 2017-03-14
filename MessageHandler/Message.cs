using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace MessageHandler
{

    public class Message
    {
        public Message(string name)
        {
            Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; private set;}
    }
}