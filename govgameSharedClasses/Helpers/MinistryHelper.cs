using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using System.Linq;

namespace govgameSharedClasses.Helpers
{
    public class MinistryHelper
    {
        public enum MinistryCode
        {
            None = -1,
            PrimeMinister = 0,
            Interior = 1,
            FinanceAndTrade = 2,
            ForeignAffairs = 3,
            Defence = 4
        }

        /// <returns>Ministry of example, Office of example</returns>
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

        /// <returns>Example Minister</returns>
        public static string MinistryCodeToMinisterName(MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    return "Prime Minister";
                case MinistryCode.Interior:
                    return "Interior Minister";
                case MinistryCode.FinanceAndTrade:
                    return "Finance and Trade Minister";
                case MinistryCode.ForeignAffairs:
                    return "Foreign Affairs Minister";
                case MinistryCode.Defence:
                    return "Defence Minister";
                default:
                    return null;
            }
        }

        /// <returns>#XXXXXX (all caps, with #)</returns>
        public static string MinistryCodeToMinistryHexColour(MinistryCode ministryCode, bool forHeader = false)
        {
            if (forHeader && ministryCode == MinistryCode.None) return "#FFFFFF";

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

        /// <returns>ministry of example.png (not absolute path, includes file extension)</returns>
        public static string MinistryCodeToMinistryImageFileName(MinistryCode ministryCode)
        {
            switch (ministryCode)
            {
                case MinistryCode.PrimeMinister:
                    return "office of the prime minister.png";
                case MinistryCode.Interior:
                    return "ministry of the interior.png";
                case MinistryCode.FinanceAndTrade:
                    return "ministry of finance and trade.png";
                case MinistryCode.ForeignAffairs:
                    return "ministry of foreign affairs.png";
                case MinistryCode.Defence:
                    return "ministry of defence.png";
                default:
                    return null;
            }
        }

        public static bool CanUserAccessMinistryDashboard(string username, string countryName, MinistryCode ministryDashboard)
        {
            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.Username == username);
                Country country = database.Countries.Single(c => c.CountryName == countryName);

                if (user.Admin) return true;

                if (user.CountryName == countryName)
                {
                    if (user.Ministry == MinistryCode.PrimeMinister) return true;
                    if (user.Ministry == ministryDashboard) return true;
                }

                return false;
            }
        }
    }
}
