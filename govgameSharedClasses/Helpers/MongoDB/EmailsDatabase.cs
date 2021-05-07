using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class EmailsDatabase
        {
            static readonly IMongoCollection<Email> emailsCollection = govgameDatabase.GetCollection<Email>("emails");
            static readonly IMongoCollection<Email> deletedEmailsCollection = govgameDatabase.GetCollection<Email>("deletedEmails");

            public static Email GetEmailById(ObjectId emailId)
            {
                FilterDefinition<Email> filter = Builders<Email>.Filter.Eq(email => email.EmailId, emailId);

                return emailsCollection.Find(filter).First();
            }

            public static Email GetEmailById(string emailIdString)
            {
                ObjectId emailId = ObjectId.Parse(emailIdString);

                return GetEmailById(emailId);
            }

            public static Email[] GetUsersReceivedEmails(string userId, bool includeBlocked = false)
            {
                FilterDefinitionBuilder<Email> filterBuilder = Builders<Email>.Filter;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(userId);

                FilterDefinition<Email> filter;
                if (includeBlocked)
                {
                    filter = filterBuilder.And(filterBuilder.Eq(email => email.RecipientId, userId));
                }
                else
                {
                    filter = filterBuilder.And(filterBuilder.Eq(email => email.RecipientId, userId), filterBuilder.Nin(email => email.SenderId, publicUser.BlockedUsers));
                }
                SortDefinition<Email> sort = Builders<Email>.Sort.Descending(email => email.EmailId);

                return emailsCollection.Find(filter).Sort(sort).ToList().ToArray();
            }

            public static bool SendEmail(EmailSendRequest emailSendRequest)
            {
                List<Email> emails = new List<Email>();

                foreach (string recipientId in emailSendRequest.RecipientIds)
                {
                    emails.Add(new Email
                    {
                        SenderId = emailSendRequest.SenderId,
                        RecipientId = recipientId,
                        RecipientIds = emailSendRequest.RecipientIds,
                        Subject = emailSendRequest.Subject,
                        Body = emailSendRequest.Body,
                        MarkedAsRead = false
                    });
                }

                try
                {
                    emailsCollection.InsertMany(emails);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            static bool MarkEmailAsReadUnread(ObjectId emailId, bool markAsRead)
            {
                FilterDefinition<Email> filter = Builders<Email>.Filter.Eq(email => email.EmailId, emailId);

                UpdateDefinition<Email> update = Builders<Email>.Update.Set(email => email.MarkedAsRead, markAsRead);

                try
                {
                    emailsCollection.UpdateOne(filter, update);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool MarkEmailAsRead(ObjectId emailId)
            {
                return MarkEmailAsReadUnread(emailId, true);
            }

            public static bool MarkEmailAsRead(string emailIdString)
            {
                ObjectId emailId = ObjectId.Parse(emailIdString);

                return MarkEmailAsRead(emailId);
            }

            public static bool MarkEmailAsUnread(ObjectId emailId)
            {
                return MarkEmailAsReadUnread(emailId, false);
            }

            public static bool MarkEmailAsUnread(string emailIdString)
            {
                ObjectId emailId = ObjectId.Parse(emailIdString);

                return MarkEmailAsUnread(emailId);
            }

            public static bool DeleteEmail(ObjectId emailId)
            {
                Email email = GetEmailById(emailId);

                FilterDefinition<Email> filter = Builders<Email>.Filter.Eq(email => email.EmailId, email.EmailId);

                try
                {
                    deletedEmailsCollection.InsertOne(email);

                    emailsCollection.DeleteOne(filter);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool DeleteEmail(string emailIdString)
            {
                ObjectId emailId = ObjectId.Parse(emailIdString);

                return DeleteEmail(emailId);
            }
        }
    }
}
