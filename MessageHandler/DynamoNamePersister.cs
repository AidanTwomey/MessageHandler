using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using System.Threading.Tasks;

namespace MessageHandler
{
    public class DynamoNamePersister : IPersister
    {
        private readonly Table namesTable;

        public DynamoNamePersister(IAmazonDynamoDB client)
        {
            namesTable = Table.LoadTable( client, "Names" );
        }

        public async Task<Document> Persist(Message message, ILambdaLogger logger)
        {
            logger.LogLine("writing " + message.Name + " to DynamoDB Names document");

            var artist = new Document();
            artist["Name"] = message.Name;
            
            return await namesTable.PutItemAsync(artist);
        }
    }
}