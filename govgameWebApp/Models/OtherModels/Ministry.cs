using govgameWebApp.Helpers;
using govgameSharedClasses.Models.MongoDB;

namespace govgameWebApp.Models.OtherModels
{
    public class Ministry
    {
        public MinistryCode Code { get; set; }
        public string ImageFileName { get; set; }
        public PublicUser MinisterPublicUser { get; set; }

        public static Ministry GetMinistry(MinistryCode ministryCode, string countryId)
        {
            Ministry ministry = new Ministry
            {
                Code = ministryCode
            };

            Country country = MongoDBHelper.GetCountry(countryId);

            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    ministry.ImageFileName = "office of the prime minister logo.png";
                    if (country.PrimeMinisterId != "none")
                    {
                        ministry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.PrimeMinisterId);
                    }
                    return ministry;

                case MinistryCode.Interior:
                    ministry.ImageFileName = "ministry of the interior.png";
                    if (country.InteriorMinisterId != "none")
                    {
                        ministry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.InteriorMinisterId);
                    }
                    return ministry;

                case MinistryCode.FinanceAndTrade:
                    ministry.ImageFileName = "ministry of finance and trade.png";
                    if (country.FinanceAndTradeMinisterId != "none")
                    {
                        ministry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.FinanceAndTradeMinisterId);
                    }
                    return ministry;

                case MinistryCode.ForeignAffairs:
                    ministry.ImageFileName = "ministry of foreign affairs.png";
                    if (country.ForeignMinisterId != "none")
                    {
                        ministry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.ForeignMinisterId);
                    }
                    return ministry;

                case MinistryCode.Defence:
                    ministry.ImageFileName = "ministry of defence.png";
                    if (country.DefenceMinisterId != "none")
                    {
                        ministry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.DefenceMinisterId);
                    }
                    return ministry;

                default:
                    return null;
            }
        }

        public static Ministry[] GetAllMinistries(string countryId)
        {
            Country country = MongoDBHelper.GetCountry(countryId);

            Ministry primeMinisterMinistry = new Ministry
            {
                Code = MinistryCode.PrimeMinister
            };
            primeMinisterMinistry.ImageFileName = "office of the prime minister logo.png";
            if (country.PrimeMinisterId != "none")
            {
                primeMinisterMinistry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.PrimeMinisterId);
            }

            Ministry interiorMinistry = new Ministry
            {
                Code = MinistryCode.Interior
            };
            interiorMinistry.ImageFileName = "ministry of the interior.png";
            if (country.InteriorMinisterId != "none")
            {
                interiorMinistry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.InteriorMinisterId);
            }

            Ministry financeAndTradeMinistry = new Ministry
            {
                Code = MinistryCode.FinanceAndTrade
            };
            financeAndTradeMinistry.ImageFileName = "ministry of finance and trade.png";
            if (country.FinanceAndTradeMinisterId != "none")
            {
                financeAndTradeMinistry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.FinanceAndTradeMinisterId);
            }

            Ministry foreignMinistry = new Ministry
            {
                Code = MinistryCode.ForeignAffairs
            };
            foreignMinistry.ImageFileName = "ministry of foreign affairs.png";
            if (country.ForeignMinisterId != "none")
            {
                foreignMinistry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.ForeignMinisterId);
            }

            Ministry defenceMinistry = new Ministry
            {
                Code = MinistryCode.Defence
            };
            defenceMinistry.ImageFileName = "ministry of defence.png";
            if (country.DefenceMinisterId != "none")
            {
                defenceMinistry.MinisterPublicUser = MongoDBHelper.GetPublicUser(country.DefenceMinisterId);
            }

            Ministry[] ministries = new Ministry[]
            {
                primeMinisterMinistry,
                interiorMinistry,
                financeAndTradeMinistry,
                foreignMinistry,
                defenceMinistry
            };

            return ministries;
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
    }
}
