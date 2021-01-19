using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
#if DEBUG
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@ec2-35-178-4-240.eu-west-2.compute.amazonaws.com");
#else
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@localhost");
#endif
    }
}
