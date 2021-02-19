using govgameSharedClasses.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace govgameSharedClasses.Models.MongoDB
{
    public class PublicUser
    {
        [BsonId]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string CountryId { get; set; }

        public bool IsAMinister()
        {
            if (this.CountryId == "none") return false;

            return true;
        }

        public bool IsAPrimeMinister()
        {
            if (this.CountryId == "none") return false;

            Country country = MongoDBHelper.CountriesDatabase.GetCountry(this.CountryId);

            if (country.GetMinisterIdByCode(MinistryHelper.MinistryCode.PrimeMinister) == this.UserId) return true;

            return false;
        }

        public MinistryHelper.MinistryCode GetMinistry()
        {
            if (this.CountryId == "none") return MinistryHelper.MinistryCode.None;

            Country country = MongoDBHelper.CountriesDatabase.GetCountry(this.CountryId);

            foreach (MinistryHelper.MinistryCode ministryCode in Enum.GetValues(typeof(MinistryHelper.MinistryCode)))
            {
                if (country.GetMinisterIdByCode(ministryCode) == this.UserId) return ministryCode;
            }

            return MinistryHelper.MinistryCode.None;
        }
    }

    public class UserUpdate
    {
        public string Username { get; set; }
        public string CountryId { get; set; }
    }
}
