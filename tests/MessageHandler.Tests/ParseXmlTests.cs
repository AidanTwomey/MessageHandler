using System;
using Xunit;
using System.Xml.Linq;
using System.IO;

namespace MessageHandler.Tests
{
    public class ParseXmlTests
    {
        [Fact]
        public void ReadName() 
        {
            var parser = new MessageParser();

            var filePath = Path.Combine( "TestData", "aDex.xml");

            Assert.Equal( "Pink Floyd", parser.Parse( XDocument.Load(filePath).ToString() ).Name );
        }
    }
}