using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class NotificationsDatabase
        {
            static readonly IMongoCollection<Notification> notificationsCollection = govgameDatabase.GetCollection<Notification>("notifications");

            public static Notification GetNotificationById(ObjectId notificationId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq(notification => notification.NotificationId, notificationId);

                return notificationsCollection.Find(filter).First();
            }

            public static Notification GetNotificationById(string notificationIdString)
            {
                ObjectId notificationId = ObjectId.Parse(notificationIdString);

                return GetNotificationById(notificationId);
            }

            public static Notification[] GetUsersReceivedNotifications(string userId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq(notification => notification.UserId, userId);

                SortDefinition<Notification> sort = Builders<Notification>.Sort.Descending(notification => notification.NotificationId);

                return notificationsCollection.Find(filter).Sort(sort).ToList().ToArray();
            }

            public static bool SendNotification(NotificationSendRequest notificationSendRequest)
            {
                Notification notification = new Notification
                {
                    UserId = notificationSendRequest.UserId,
                    Title = notificationSendRequest.Title,
                    Content = notificationSendRequest.Content,
                    Link = notificationSendRequest.Link,
                    MarkedAsRead = false
                };

                try
                {
                    notificationsCollection.InsertOne(notification);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool MarkNotificationAsRead(ObjectId notificationId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq(notification => notification.NotificationId, notificationId);

                UpdateDefinition<Notification> update = Builders<Notification>.Update.Set(notification => notification.MarkedAsRead, true);

                try
                {
                    notificationsCollection.UpdateOne(filter, update);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool MarkNotificationAsRead(string notificationIdString)
            {
                ObjectId notificationId = ObjectId.Parse(notificationIdString);

                return MarkNotificationAsRead(notificationId);
            }
        }
    }
}
