using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Notification
    {
        [BsonId]
        public ObjectId NotificationId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool MarkedAsRead { get; set; }
    }

    public class NotificationSendRequest
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
    }
}
