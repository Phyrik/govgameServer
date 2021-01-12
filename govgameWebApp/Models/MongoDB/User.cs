using MongoDB.Bson.Serialization.Attributes;

namespace govgameWebApp.Models.MongoDB
{
    public class PublicUser
    {
        [BsonId]
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool OwnsCountry { get; set; }
        public bool IsMinister { get; set; }
        public string CountryId { get; set; }
    }

    public class UserUpdate
    {
        public string Username { get; set; }
        public bool OwnsCountry { get; set; }
        public bool IsMinister { get; set; }
        public string CountryId { get; set; }
    }
}
