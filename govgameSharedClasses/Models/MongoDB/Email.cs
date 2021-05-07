using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Email
    {
        [BsonId]
        public ObjectId EmailId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string[] RecipientIds { get; set; } // ONLY used to show user who all the email was originally sent to. Use RecipientId to identify who the email is meant to be for
        public string Subject { get; set; } // plain text
        public string Body { get; set; } // can be styled with markdown or html
        public bool MarkedAsRead { get; set; }

        // reminder: NEVER trust user input
        // when adding new properties, remember to update the schema
    }

    public class EmailSendRequest
    {
        public string SenderId { get; set; }
        public string[] RecipientIds { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
