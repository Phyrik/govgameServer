using govgameSharedClasses.Models.MongoDB;
using MongoDB.Driver;

namespace govgameSharedClasses.Helpers
{
    public class CountryBudgetHelper
    {
        public static bool TransferMoneyToFromMinisterialBudget(string countryId, MinistryHelper.MinistryCode ministryCode, int amount)
        {
            if (ministryCode == MinistryHelper.MinistryCode.PrimeMinister || ministryCode == MinistryHelper.MinistryCode.None || ministryCode == MinistryHelper.MinistryCode.FinanceAndTrade) return false;

            string ministryBudgetFieldName;
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.Interior:
                    ministryBudgetFieldName = "InteriorMinistryBudget";
                    break;

                case MinistryHelper.MinistryCode.ForeignAffairs:
                    ministryBudgetFieldName = "ForeignMinistryBudget";
                    break;

                case MinistryHelper.MinistryCode.Defence:
                    ministryBudgetFieldName = "DefenceMinistryBudget";
                    break;

                default:
                    ministryBudgetFieldName = null;
                    break;
            }

            UpdateDefinitionBuilder<Country> updateBuilder = Builders<Country>.Update;

            UpdateDefinition<Country> update = updateBuilder.Combine(updateBuilder.Inc(ministryBudgetFieldName, amount), updateBuilder.Inc("SpareBudget", -amount));

            FilterDefinitionBuilder<Country> filterBuilder = Builders<Country>.Filter;

            FilterDefinition<Country> abortFilter = filterBuilder.And(filterBuilder.Gte(ministryBudgetFieldName, -amount), filterBuilder.Gte("SpareBudget", amount));

            return MongoDBHelper.CountriesDatabase.UpdateCountryRaw(countryId, update, abortFilter);
        }
    }
}
