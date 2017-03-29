using System;
using Xunit;
using System.Xml.Linq;
using System.IO;
using NSubstitute;
using Amazon.Lambda.Core;

namespace MessageHandler.Tests
{
    public class ParseXmlTests
    {
        [Fact]
        public void ReadName() 
        {
            var parser = new MessageParser();

            var filePath = Path.Combine( "TestData", "aDex.xml");

            var parsedFile = parser.Parse( XDocument.Load(filePath).ToString());

            Assert.Equal( "Pink Floyd", parsedFile.Name );
            Assert.Equal( "Another Brick In The Wall (Part 2)", parsedFile.Source );
        }
    }
}