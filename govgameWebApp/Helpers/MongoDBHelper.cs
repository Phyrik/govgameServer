using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace govgameWebApp.Helpers
{
    public class MongoDBHelper
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
#if DEBUG
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@ec2-35-178-4-240.eu-west-2.compute.amazonaws.com");
#else
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@localhost");
#endif

        #region Map Database
        static readonly IMongoDatabase mapDatabase = mongoClient.GetDatabase("govgame_map_table");
        static readonly IMongoCollection<Location> locationsCollection = mapDatabase.GetCollection<Location>("locations");

        public static FilterDefinition<Location> GetDBFilterForLocationIdentifier(GlobalLocationIdentifier globalLocationIdentifier)
        {
            FilterDefinitionBuilder<Location> builder = Builders<Location>.Filter;
            FilterDefinition<Location> filter = builder.And(builder.Eq("GlobalX", globalLocationIdentifier.GlobalX), builder.Eq("GlobalY", globalLocationIdentifier.GlobalY));

            return filter;
        }

        public static FilterDefinition<Location> GetDBFilterForLocationIdentifier(RegionalLocationIdentifier regionalLocationIdentifier)
        {
            FilterDefinitionBuilder<Location> builder = Builders<Location>.Filter;
            FilterDefinition<Location> filter = builder.And(builder.Eq("RegionId", regionalLocationIdentifier.RegionId), builder.Eq("RegionalX", regionalLocationIdentifier.RegionalX), builder.Eq("RegionalY", regionalLocationIdentifier.RegionalY));

            return filter;
        }

        public static Map GetWorldMap()
        {
            Map worldMap = new Map();
            worldMap.Regions = new List<Region>();
            foreach (char regionChar1 in MapHelper.worldRegionChars)
            {
                foreach (char regionChar2 in MapHelper.worldRegionChars)
                {
                    worldMap.Regions.Add(GetRegion($"{regionChar1}{regionChar2}"));
                }
            }

            return worldMap;
        }

        public static Region GetRegion(string regionId)
        {
            FilterDefinition<Location> filter = Builders<Location>.Filter.Eq("RegionId", regionId);

            List<Location> locations = locationsCollection.Find(filter).ToList();

            return new Region
            {
                RegionId = regionId,
                Locations = locations
            };
        }

        public static Location GetLocation(GlobalLocationIdentifier globalLocationIdentifier)
        {
            FilterDefinition<Location> filter = GetDBFilterForLocationIdentifier(globalLocationIdentifier);

            return locationsCollection.Find(filter).First();
        }

        public static Location GetLocation(RegionalLocationIdentifier regionalLocationIdentifier)
        {
            FilterDefinition<Location> filter = GetDBFilterForLocationIdentifier(regionalLocationIdentifier);

            return locationsCollection.Find(filter).First();
        }

        public static Location[] GetLocations(GlobalLocationIdentifier topLeftGlobalLocationIdentifier, int width, int height)
        {
            FilterDefinitionBuilder<Location> filterBuilder = Builders<Location>.Filter;
            FilterDefinition<Location> filter = filterBuilder.And(filterBuilder.Gte("GlobalX", topLeftGlobalLocationIdentifier.GlobalX), filterBuilder.Lt("GlobalX", topLeftGlobalLocationIdentifier.GlobalX + width), filterBuilder.Gte("GlobalY", topLeftGlobalLocationIdentifier.GlobalY), filterBuilder.Lt("GlobalY", topLeftGlobalLocationIdentifier.GlobalY + height));

            try
            {
                return locationsCollection.Find(filter).ToList().ToArray();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Not implemented yet</summary>
        public static Location[] GetLocations(GlobalLocationIdentifier[] globalLocationIdentifiers)
        {
            throw new NotImplementedException();
        }

        /// <summary>Not implemented yet</summary>
        public static Location[] GetLocations(RegionalLocationIdentifier[] regionalLocationIdentifiers)
        {
            throw new NotImplementedException();
        }

        public static bool UpdateLocations(GlobalLocationIdentifier topLeftGlobalLocationIdentifier, int width, int height, LocationUpdate locationUpdate)
        {
            FilterDefinitionBuilder<Location> filterBuilder = Builders<Location>.Filter;
            FilterDefinition<Location> filter = filterBuilder.And(filterBuilder.Gte("GlobalX", topLeftGlobalLocationIdentifier.GlobalX), filterBuilder.Lt("GlobalX", topLeftGlobalLocationIdentifier.GlobalX + width), filterBuilder.Gte("GlobalY", topLeftGlobalLocationIdentifier.GlobalY), filterBuilder.Lt("GlobalY", topLeftGlobalLocationIdentifier.GlobalY + height));

            UpdateDefinitionBuilder<Location> updateBuilder = Builders<Location>.Update;
            List<UpdateDefinition<Location>> updates = new List<UpdateDefinition<Location>>();
            PropertyInfo[] properties = typeof(LocationUpdate).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(locationUpdate) != null)
                {
                    updates.Add(updateBuilder.Set(property.Name, property.GetValue(locationUpdate)));
                }
            }

            try
            {
                locationsCollection.UpdateMany(filter, updateBuilder.Combine(updates));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Not implemented yet</summary>
        public static bool UpdateLocations(GlobalLocationIdentifier[] globalLocationIdentifiers, LocationUpdate locationUpdate)
        {
            throw new NotImplementedException();
        }

        /// <summary>Not implemented yet</summary>
        public static bool UpdateLocations(RegionalLocationIdentifier[] regionalLocationIdentifiers, LocationUpdate locationUpdate)
        {
            throw new NotImplementedException();
        }

        public static Location[] GetCountrysLocations(string countryId)
        {
            FilterDefinition<Location> filter = Builders<Location>.Filter.Eq("Owner", countryId);

            return locationsCollection.Find(filter).ToList().ToArray();
        }
        #endregion

        #region Users Database
        static readonly IMongoDatabase usersDatabase = mongoClient.GetDatabase("govgame_user_table");
        static readonly IMongoCollection<PublicUser> usersCollection = usersDatabase.GetCollection<PublicUser>("users");

        public static PublicUser GetPublicUser(string userId)
        {
            FilterDefinition<PublicUser> filter = Builders<PublicUser>.Filter.Eq("UserId", userId);

            return usersCollection.Find(filter).First();
        }

        public static bool UpdateUser(string userId, UserUpdate userUpdate)
        {
            FilterDefinition<PublicUser> filter = Builders<PublicUser>.Filter.Eq("UserId", userId);

            UpdateDefinitionBuilder<PublicUser> updateBuilder = Builders<PublicUser>.Update;
            List<UpdateDefinition<PublicUser>> updates = new List<UpdateDefinition<PublicUser>>();
            PropertyInfo[] properties = typeof(UserUpdate).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(userUpdate) != null)
                {
                    updates.Add(updateBuilder.Set(property.Name, property.GetValue(userUpdate)));
                }
            }

            try
            {
                usersCollection.UpdateMany(filter, updateBuilder.Combine(updates));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool NewUser(PublicUser publicUser)
        {
            try
            {
                usersCollection.InsertOne(publicUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static PublicUser[] GetAllPublicUsers(bool excludeMinisters = false)
        {
            FilterDefinition<PublicUser> filter = Builders<PublicUser>.Filter.Empty;

            PublicUser[] allPublicUsers = usersCollection.Find(filter).ToList().ToArray();

            if (excludeMinisters)
            {
                List<PublicUser> allPublicUsersNotMinisters = new List<PublicUser>();
                foreach (PublicUser publicUser in allPublicUsers)
                {
                    if (!publicUser.OwnsCountry && !publicUser.IsMinister)
                    {
                        allPublicUsersNotMinisters.Add(publicUser);
                    }
                }

                return allPublicUsersNotMinisters.ToArray();
            }
            else
            {
                return allPublicUsers;
            }
        }

        public static string[] GetAllUsernames()
        {
            FilterDefinition<PublicUser> filter = Builders<PublicUser>.Filter.Empty;

            PublicUser[] users = usersCollection.Find(filter).ToList().ToArray();

            List<string> usernames = new List<string>();
            foreach (PublicUser user in users)
            {
                usernames.Add(user.Username);
            }

            return usernames.ToArray();
        }
        #endregion

        #region Countries Database
        static readonly IMongoDatabase countriesDatabase = mongoClient.GetDatabase("govgame_countries_table");
        static readonly IMongoCollection<Country> countriesCollection = countriesDatabase.GetCollection<Country>("countries");

        public static Country GetCountry(string countryId)
        {
            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq("CountryId", countryId);

            return countriesCollection.Find(filter).First();
        }

        public static bool UpdateCountry(string countryId, CountryUpdate countryUpdate)
        {
            FilterDefinition<Country> filter = Builders<Country>.Filter.Eq("CountryId", countryId);

            UpdateDefinitionBuilder<Country> updateBuilder = Builders<Country>.Update;
            List<UpdateDefinition<Country>> updates = new List<UpdateDefinition<Country>>();
            PropertyInfo[] properties = typeof(CountryUpdate).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(countryUpdate) != null)
                {
                    updates.Add(updateBuilder.Set(property.Name, property.GetValue(countryUpdate)));
                }
            }

            try
            {
                countriesCollection.UpdateMany(filter, updateBuilder.Combine(updates));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool NewCountry(Country country)
        {
            try
            {
                countriesCollection.InsertOne(country);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] GetAllCountryIds()
        {
            FilterDefinition<Country> filter = Builders<Country>.Filter.Empty;

            Country[] countries = countriesCollection.Find(filter).ToList().ToArray();

            List<string> countryIds = new List<string>();
            foreach (Country country in countries)
            {
                countryIds.Add(country.CountryId);
            }

            return countryIds.ToArray();
        }
        #endregion

        #region Emails Database
        static readonly IMongoDatabase emailsDatabase = mongoClient.GetDatabase("govgame_emails_table");
        static readonly IMongoCollection<Email> emailsCollection = emailsDatabase.GetCollection<Email>("emails");
        static readonly IMongoCollection<Email> deletedEmailsCollection = emailsDatabase.GetCollection<Email>("deletedEmails");

        public static Email GetEmailById(ObjectId emailId)
        {
            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq("EmailId", emailId);

            return emailsCollection.Find(filter).First();
        }

        public static Email GetEmailById(string emailIdString)
        {
            ObjectId emailId = ObjectId.Parse(emailIdString);

            return GetEmailById(emailId);
        }

        public static Email[] GetUsersReceivedEmails(string userId)
        {
            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq("RecipientId", userId);

            return emailsCollection.Find(filter).ToList().ToArray();
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

        public static bool MarkEmailAsRead(ObjectId emailId)
        {
            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq("EmailId", emailId);

            UpdateDefinition<Email> update = Builders<Email>.Update.Set("MarkedAsRead", true);

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

        public static bool MarkEmailAsRead(string emailIdString)
        {
            ObjectId emailId = ObjectId.Parse(emailIdString);

            return MarkEmailAsRead(emailId);
        }

        public static bool MarkEmailAsUnread(ObjectId emailId)
        {
            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq("EmailId", emailId);

            UpdateDefinition<Email> update = Builders<Email>.Update.Set("MarkedAsRead", false);

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

        public static bool MarkEmailAsUnread(string emailIdString)
        {
            ObjectId emailId = ObjectId.Parse(emailIdString);

            return MarkEmailAsUnread(emailId);
        }

        public static bool DeleteEmail(ObjectId emailId)
        {
            Email email = GetEmailById(emailId);

            FilterDefinition<Email> filter = Builders<Email>.Filter.Eq("EmailId", email.EmailId);

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
        #endregion

        #region Notifications Database
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

            return notificationsCollection.Find(filter).ToList().ToArray();
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
        #endregion
    }
}
