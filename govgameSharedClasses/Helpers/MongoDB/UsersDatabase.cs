using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Reflection;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class UsersDatabase
        {
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
        }
    }
}
