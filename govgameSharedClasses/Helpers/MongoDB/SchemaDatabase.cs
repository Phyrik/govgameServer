using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class SchemaDatabase
        {
            static readonly IMongoCollection<Schema> schemasCollection = govgameDatabase.GetCollection<Schema>("schemas");

            public static Schema GetSchema(string collectionName)
            {
                FilterDefinition<Schema> filter = Builders<Schema>.Filter.Eq(schema => schema.CollectionName, collectionName);

                return schemasCollection.Find(filter).First();
            }
        }
    }
}