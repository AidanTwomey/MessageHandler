using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.DocumentModel;
using System.Threading.Tasks;

namespace MessageHandler
{
    public interface IPersister
    {
        Task<Document> Persist(Message message, ILambdaLogger logger);
    }
}