using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class LocationsDatabase
        {
            static readonly IMongoCollection<Location> locationsCollection = govgameDatabase.GetCollection<Location>("locations");

            static FilterDefinition<Location> GetDBFilterForLocationIdentifier(GlobalLocationIdentifier globalLocationIdentifier)
            {
                FilterDefinitionBuilder<Location> builder = Builders<Location>.Filter;
                FilterDefinition<Location> filter = builder.And(builder.Eq("GlobalX", globalLocationIdentifier.GlobalX), builder.Eq("GlobalY", globalLocationIdentifier.GlobalY));

                return filter;
            }

            public static Location GetLocation(GlobalLocationIdentifier globalLocationIdentifier)
            {
                FilterDefinition<Location> filter = GetDBFilterForLocationIdentifier(globalLocationIdentifier);

                return locationsCollection.Find(filter).First();
            }

            public static Location[] GetLocations(GlobalLocationIdentifier topLeftGlobalLocationIdentifier, int width, int height)
            {
                FilterDefinitionBuilder<Location> filterBuilder = Builders<Location>.Filter;
                FilterDefinition<Location> filter = filterBuilder.And(filterBuilder.Gte("GlobalX", topLeftGlobalLocationIdentifier.GlobalX), filterBuilder.Lt("GlobalX", topLeftGlobalLocationIdentifier.GlobalX + width), filterBuilder.Gte("GlobalY", topLeftGlobalLocationIdentifier.GlobalY), filterBuilder.Lt("GlobalY", topLeftGlobalLocationIdentifier.GlobalY + height));

                return locationsCollection.Find(filter).ToList().ToArray();
            }

            public static bool UpdateLocation(GlobalLocationIdentifier globalLocationIdentifier, LocationUpdate locationUpdate)
            {
                FilterDefinitionBuilder<Location> filterBuilder = Builders<Location>.Filter;
                FilterDefinition<Location> filter = filterBuilder.And(filterBuilder.Eq("GlobalX", globalLocationIdentifier.GlobalX), filterBuilder.Eq("GlobalY", globalLocationIdentifier.GlobalY));

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
                    locationsCollection.UpdateOne(filter, updateBuilder.Combine(updates));

                    return true;
                }
                catch
                {
                    return false;
                }
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
                    locationsCollection.UpdateOne(filter, updateBuilder.Combine(updates));

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static Location[] GetCountrysLocations(string countryId)
            {
                FilterDefinition<Location> filter = Builders<Location>.Filter.Eq("Owner", countryId);

                return locationsCollection.Find(filter).ToList().ToArray();
            }
        }
    }
}
