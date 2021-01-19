using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class MapDatabase
        {
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
        }
    }
}
