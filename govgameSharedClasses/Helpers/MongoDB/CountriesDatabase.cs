﻿using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Reflection;

namespace govgameSharedClasses.Helpers
{
    public partial class MongoDBHelper
    {
        public class CountriesDatabase
        {
            static readonly IMongoCollection<Country> countriesCollection = govgameDatabase.GetCollection<Country>("countries");

            public static Country GetCountry(string countryId)
            {
                FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(country => country.CountryId, countryId);

                return countriesCollection.Find(filter).First();
            }

            public static bool UpdateCountry(string countryId, CountryUpdate countryUpdate)
            {
                if (countryUpdate.DeleteCountry.HasValue && countryUpdate.DeleteCountry.Value == true)
                {
                    return DeleteCountry(countryId);
                }

                FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(country => country.CountryId, countryId);

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
                    countriesCollection.UpdateOne(filter, updateBuilder.Combine(updates));

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool UpdateCountryRaw(string countryId, UpdateDefinition<Country> update, FilterDefinition<Country> filter = null)
            {
                FilterDefinitionBuilder<Country> filterBuilder = Builders<Country>.Filter;

                if (filter == null)
                {
                    filter = filterBuilder.Empty;
                }

                FilterDefinition<Country> finalFilter = filterBuilder.And(filterBuilder.Eq(country => country.CountryId, countryId), filter);

                try
                {
                    countriesCollection.UpdateOne(finalFilter, update);

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

            public static bool DeleteCountry(string countryId)
            {
                FilterDefinition<Country> filter = Builders<Country>.Filter.Eq(country => country.CountryId, countryId);

                try
                {
                    countriesCollection.DeleteOne(filter);

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

            public static string[] GetAllCountryNames()
            {
                FilterDefinition<Country> filter = Builders<Country>.Filter.Empty;

                Country[] countries = countriesCollection.Find(filter).ToList().ToArray();

                List<string> countryNames = new List<string>();
                foreach (Country country in countries)
                {
                    countryNames.Add(country.CountryName);
                }

                return countryNames.ToArray();
            }
        }
    }
}
