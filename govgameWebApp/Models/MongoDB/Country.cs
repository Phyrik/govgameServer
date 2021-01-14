using MongoDB.Bson.Serialization.Attributes;

namespace govgameWebApp.Models.MongoDB
{
    public class Country
    {
        [BsonId]
        public string CountryId { get; set; }
        public string CountryName { get; set; }
        public string CapitalName { get; set; }
        public int FlagId { get; set; }
        public string PrimeMinisterId { get; set; }
        public string InteriorMinisterId { get; set; }
        public string FinanceAndTradeMinisterId { get; set; }
        public string ForeignMinisterId { get; set; }
        public string DefenceMinisterId { get; set; }
    }

    public class CountryUpdate
    {
        public string CountryName { get; set; }
        public string CapitalName { get; set; }
        public int? FlagId { get; set; } = null;
        public string PrimeMinisterId { get; set; }
        public string InteriorMinisterId { get; set; }
        public string FinanceAndTradeMinisterId { get; set; }
        public string ForeignMinisterId { get; set; }
        public string DefenceMinisterId { get; set; }
    }
}
