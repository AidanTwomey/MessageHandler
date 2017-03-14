using System;
using System.Xml.Linq;
using System.Linq;
using Amazon.Lambda.Core;

namespace MessageHandler
{
    public class MessageParser
    {
        [LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public Message Parse(string xmlMessage)
        {    
            var message = XDocument.Parse(xmlMessage);

            XElement name = message.Descendants("Name").Single();

            return new Message( name.Element("FirstName").Value + " " + name.Element("Surname").Value );
        }
    }
}