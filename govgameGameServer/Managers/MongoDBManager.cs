using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;

namespace govgameGameServer.Managers
{
    public static class MongoDBManager
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@localhost");

        #region Notification Database
        static readonly IMongoDatabase notificationsDatabase = mongoClient.GetDatabase("govgame_notifications_table");
        static readonly IMongoCollection<Notification> notificationsCollection = notificationsDatabase.GetCollection<Notification>("notifications");

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
        #endregion
    }
}
