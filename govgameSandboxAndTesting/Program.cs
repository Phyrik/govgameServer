using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using System;
using System.Linq;

namespace govgameSandboxAndTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var database = new DatabaseContext())
            {
                User phyrik = database.Users.Where(user => user.Username == "phyrik").First();
                Console.WriteLine(phyrik.CountryName);
            }
        }
    }
}
