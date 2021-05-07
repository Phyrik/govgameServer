using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Schema
    {
        [BsonId]
        public string CollectionName { get; set; }
        public string[] UserReferences { get; set; }
        public string[] UserReferenceArrays { get; set; }
        public string[] CountryReferences { get; set; }
        public string[] CountryReferenceArrays { get; set; }
    }
}