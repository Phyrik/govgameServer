using govgameSharedClasses.Helpers;
using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    [BsonIgnoreExtraElements]
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
        [BsonDefaultValue(0)]
        public int Budget { get; set; }
        [BsonDefaultValue(0)]
        public int InteriorMinistryBudget { get; set; }
        [BsonDefaultValue(0)]
        public int ForeignMinistryBudget { get; set; }
        [BsonDefaultValue(0)]
        public int DefenceMinistryBudget { get; set; }

        public string GetMinisterIdByCode(MinistryHelper.MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.PrimeMinister:
                    return this.PrimeMinisterId;
                case MinistryHelper.MinistryCode.Interior:
                    return this.InteriorMinisterId;
                case MinistryHelper.MinistryCode.FinanceAndTrade:
                    return this.FinanceAndTradeMinisterId;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    return this.ForeignMinisterId;
                case MinistryHelper.MinistryCode.Defence:
                    return this.DefenceMinisterId;
                default:
                    return null;
            }
        }

        public string GetInvitedMinisterIdByCode(MinistryHelper.MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.Interior:
                    return this.InvitedInteriorMinisterId;
                case MinistryHelper.MinistryCode.FinanceAndTrade:
                    return this.InvitedFinanceAndTradeMinisterId;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    return this.InvitedForeignMinisterId;
                case MinistryHelper.MinistryCode.Defence:
                    return this.InvitedDefenceMinisterId;
                default:
                    return null;
            }
        }

        public int? GetBudgetByCode(MinistryHelper.MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.Interior:
                    return this.InteriorMinistryBudget;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    return this.ForeignMinistryBudget;
                case MinistryHelper.MinistryCode.Defence:
                    return this.DefenceMinistryBudget;
                default:
                    return null;
            }
        }
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
        public int? Budget { get; set; } = null;
        public int? InteriorMinistryBudget { get; set; } = null;
        public int? ForeignMinistryBudget { get; set; } = null;
        public int? DefenceMinistryBudget { get; set; } = null;

        public bool? DeleteCountry { get; set; } = null;

        public void SetMinisterIdByCode(MinistryHelper.MinistryCode ministryCode, string ministerId)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.PrimeMinister:
                    this.PrimeMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.Interior:
                    this.InteriorMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.FinanceAndTrade:
                    this.FinanceAndTradeMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    this.ForeignMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.Defence:
                    this.DefenceMinisterId = ministerId;
                    break;
            }
        }

        public void SetInvitedMinisterIdByCode(MinistryHelper.MinistryCode ministryCode, string ministerId)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.Interior:
                    this.InvitedInteriorMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.FinanceAndTrade:
                    this.InvitedFinanceAndTradeMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    this.InvitedForeignMinisterId = ministerId;
                    break;
                case MinistryHelper.MinistryCode.Defence:
                    this.InvitedDefenceMinisterId = ministerId;
                    break;
            }
        }

        public void SetBudgetByCode(MinistryHelper.MinistryCode ministryCode, int amount)
        {
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.Interior:
                    this.InteriorMinistryBudget = amount;
                    break;
                case MinistryHelper.MinistryCode.ForeignAffairs:
                    this.ForeignMinistryBudget = amount;
                    break;
                case MinistryHelper.MinistryCode.Defence:
                    this.DefenceMinistryBudget = amount;
                    break;
            }
        }
    }
}
