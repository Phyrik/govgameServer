using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    public class PublicUser
    {
        [BsonId]
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool OwnsCountry { get; set; }
        public bool IsMinister { get; set; }
        /// <summary>
        /// This integer represents a value from the MinistryHelper.MinistryCode enum
        /// </summary>
        [BsonDefaultValue(-1)]
        public int Ministry { get; set; }
        public string CountryId { get; set; }
    }

    public class UserUpdate
    {
        public string Username { get; set; }
        public bool OwnsCountry { get; set; }
        public bool IsMinister { get; set; }
        /// <summary>
        /// This integer represents a value from the MinistryHelper.MinistryCode enum
        /// </summary>
        public int Ministry { get; set; }
        public string CountryId { get; set; }
    }
}
