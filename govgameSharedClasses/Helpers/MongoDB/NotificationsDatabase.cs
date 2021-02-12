﻿using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class NotificationsDatabase
        {
            static readonly IMongoDatabase notificationsDatabase = mongoClient.GetDatabase("govgame_notifications_table");
            static readonly IMongoCollection<Notification> notificationsCollection = notificationsDatabase.GetCollection<Notification>("notifications");

            public static Notification GetNotificationById(ObjectId notificationId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq("NotificationId", notificationId);

                return notificationsCollection.Find(filter).First();
            }

            public static Notification GetNotificationById(string notificationIdString)
            {
                ObjectId notificationId = ObjectId.Parse(notificationIdString);

                return GetNotificationById(notificationId);
            }

            public static Notification[] GetUsersReceivedNotifications(string userId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq("UserId", userId);

                SortDefinition<Notification> sort = Builders<Notification>.Sort.Descending("NotificationId");

                return notificationsCollection.Find(filter).Sort(sort).ToList().ToArray();
            }

            public static bool MarkNotificationAsRead(ObjectId notificationId)
            {
                FilterDefinition<Notification> filter = Builders<Notification>.Filter.Eq("NotificationId", notificationId);

                UpdateDefinition<Notification> update = Builders<Notification>.Update.Set("MarkedAsRead", true);

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