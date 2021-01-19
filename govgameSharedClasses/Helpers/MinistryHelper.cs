namespace govgameSharedClasses.Helpers
{
    public class MinistryHelper
    {
        public enum MinistryCode
        {
            PrimeMinister,
            Interior,
            FinanceAndTrade,
            ForeignAffairs,
            Defence
        }

        /// <returns>Ministry of blank, Office of blank</returns>
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

        /// <returns>Blank Minister</returns>
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
                    return "Foreign Minister";
                case MinistryCode.Defence:
                    return "Defence Minister";
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
                    return "ministry of defence.png";
                default:
                    return null;
            }
        }
    }
}

