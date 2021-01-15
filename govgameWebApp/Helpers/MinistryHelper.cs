using govgameSharedClasses.Models.MongoDB;

namespace govgameWebApp.Helpers
{
    public class MinistryHelper
    {
        public static PublicUser GetPublicUserForCountryByMinistryCode(string countryId, MinistryCode ministryCode)
        {
            Country country = MongoDBHelper.GetCountry(countryId);

            PublicUser minister;
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    if (country.PrimeMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.PrimeMinisterId);
                    return minister;
                case MinistryCode.Interior:
                    if (country.InteriorMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.InteriorMinisterId);
                    return minister;
                case MinistryCode.FinanceAndTrade:
                    if (country.FinanceAndTradeMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.FinanceAndTradeMinisterId);
                    return minister;
                case MinistryCode.ForeignAffairs:
                    if (country.ForeignMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.ForeignMinisterId);
                    return minister;
                case MinistryCode.Defence:
                    if (country.DefenceMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.DefenceMinisterId);
                    return minister;
                default:
                    return null;
            }
        }

        public static PublicUser GetInvitedPublicUserForCountryByMinistryCode(string countryId, MinistryCode ministryCode)
        {
            Country country = MongoDBHelper.GetCountry(countryId);

            PublicUser minister;
            switch (ministryCode)
            {
                case MinistryCode.Interior:
                    if (country.InvitedInteriorMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.InvitedInteriorMinisterId);
                    return minister;
                case MinistryCode.FinanceAndTrade:
                    if (country.InvitedFinanceAndTradeMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.InvitedFinanceAndTradeMinisterId);
                    return minister;
                case MinistryCode.ForeignAffairs:
                    if (country.InvitedForeignMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.InvitedForeignMinisterId);
                    return minister;
                case MinistryCode.Defence:
                    if (country.InvitedDefenceMinisterId == "none")
                    {
                        return null;
                    }
                    minister = MongoDBHelper.GetPublicUser(country.InvitedDefenceMinisterId);
                    return minister;
                default:
                    return null;
            }
        }

        public enum MinistryCode
        {
            PrimeMinister,
            Interior,
            FinanceAndTrade,
            ForeignAffairs,
            Defence
        }

        public static string MinistryCodeToMinistryName(MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    return "Office of the Prime Minister";
                case MinistryCode.Interior:
                    return "Ministry of the Interior";
                case MinistryCode.FinanceAndTrade:
                    return "Ministry of Finance and Trade";
                case MinistryCode.ForeignAffairs:
                    return "Ministry of Foreign Affairs";
                case MinistryCode.Defence:
                    return "Ministry of Defence";
                default:
                    return null;
            }
        }

        /// <returns>#XXXXXX (all caps, with #)</returns>
        public static string MinistryCodeToMinistryHexColour(MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    return "#000000";
                case MinistryCode.Interior:
                    return "#8A0083";
                case MinistryCode.FinanceAndTrade:
                    return "#1F631F";
                case MinistryCode.ForeignAffairs:
                    return "#090D75";
                case MinistryCode.Defence:
                    return "#61271B";
                default:
                    return null;
            }
        }

        /// <returns>ministry of example.png (not absolute path, inculdes file extension)</returns>
        public static string MinistryCodeToMinistryImageFileName(MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    return "office of the prime minister logo.png";
                case MinistryCode.Interior:
                    return "ministry of the interior.png";
                case MinistryCode.FinanceAndTrade:
                    return "ministry of finance and trade.png";
                case MinistryCode.ForeignAffairs:
                    return "ministry of foreign affairs.png";
                case MinistryCode.Defence:
                    return "ministry of defence.pn";
                default:
                    return null;
            }
        }
    }
}

