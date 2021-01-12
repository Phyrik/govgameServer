using System;
using System.Linq;

namespace govgameWebApp.Helpers
{
    public class CountryGenerationHelper
    {
        public static readonly string[] flagNames = new string[] { "default", "france", "germany", "india", "russia", "uk", "usa" };

        public static string GenerateCountryUUID()
        {
            string[] existingUUIDs = MongoDBHelper.GetAllCountryIds();

            int uuidLength = 10;
            string uuid = GenerateUUID(uuidLength);

            while (existingUUIDs.Contains(uuid))
            {
                uuid = GenerateUUID(uuidLength);
            }

            return uuid;
        }

        public static string GenerateUUID(int length)
        {
            Random random = new Random();
            string chars = "abcdefghijklmnopgrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int FlagNameToId(string flagName)
        {
            Console.WriteLine(flagName);
            Console.WriteLine(Array.IndexOf(flagNames, flagName));
            return Array.IndexOf(flagNames, flagName);
        }
    }
}
