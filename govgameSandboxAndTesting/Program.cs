using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace govgameSandboxAndTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var database = new MySQLHelper.DatabaseContext())
            {
                List<Country> countries = database.countries.ToList();

                Console.WriteLine(countries[0].CountryName);
            }
        }
    }
}
