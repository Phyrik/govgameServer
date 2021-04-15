using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        static readonly MongoClient mongoClient = new MongoClient($"mongodb+srv://adminUser:NnMAJ%40KNJ4q652z@govgame.zelqn.mongodb.net/govgame?retryWrites=true&w=majority");
        static readonly IMongoDatabase govgameDatabase = mongoClient.GetDatabase("govgame");
    }
}
