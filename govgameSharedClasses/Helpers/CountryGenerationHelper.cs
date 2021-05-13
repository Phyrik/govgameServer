using System;

namespace govgameSharedClasses.Helpers
{
    public class CountryGenerationHelper
    {
        public static readonly string[] flagNames = new string[] { "default", "france", "germany", "india", "russia", "uk", "usa", "italy" };

        public static int FlagNameToId(string flagName)
        {
            return Array.IndexOf(flagNames, flagName);
        }
    }
}
