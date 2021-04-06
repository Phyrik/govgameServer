using govgameSharedClasses.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace govgameSharedClasses.Models.MongoDB
{
    public class PublicUser
    {
        [BsonId]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string CountryId { get; set; }
        [BsonDefaultValue(new string[] { "none" })]
        public string[] BlockedUsers { get; set; }
        [BsonDefaultValue(false)]
        public bool Admin { get; set; }

        // when adding new properties, remember to set them to their defaults in AuthController RegisterPOST

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

        public bool HasAccessToMinistry(MinistryHelper.MinistryCode ministryCode)
        {
            if (this.Admin) return true;

            if (this.IsAPrimeMinister()) return true;

            if (this.GetMinistry() == ministryCode) return true;

            return false;
        }
    }

    public class UserUpdate
    {
        public string Username { get; set; }
        public string CountryId { get; set; }
        public string[] BlockedUsers { get; set; }
    }
}
