using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
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
        [BsonDefaultValue("none")]
        public string InvitedInteriorMinisterId { get; set; }
        public string FinanceAndTradeMinisterId { get; set; }
        [BsonDefaultValue("none")]
        public string InvitedFinanceAndTradeMinisterId { get; set; }
        public string ForeignMinisterId { get; set; }
        [BsonDefaultValue("none")]
        public string InvitedForeignMinisterId { get; set; }
        public string DefenceMinisterId { get; set; }
        [BsonDefaultValue("none")]
        public string InvitedDefenceMinisterId { get; set; }
    }

    public class CountryUpdate
    {
        public string CountryName { get; set; }
        public string CapitalName { get; set; }
        public int? FlagId { get; set; } = null;
        public string PrimeMinisterId { get; set; }
        public string InteriorMinisterId { get; set; }
        public string InvitedInteriorMinisterId { get; set; }
        public string FinanceAndTradeMinisterId { get; set; }
        public string InvitedFinanceAndTradeMinisterId { get; set; }
        public string ForeignMinisterId { get; set; }
        public string InvitedForeignMinisterId { get; set; }
        public string DefenceMinisterId { get; set; }
        public string InvitedDefenceMinisterId { get; set; }
    }
}
