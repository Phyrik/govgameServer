using System;
using govgameGameServer.Managers;

namespace govgameGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting the govgameServer");

            Console.WriteLine("Starting the govgameGameServer");
            StartAllManagers();

            Console.WriteLine("Starting the govgameWebApp");
            govgameWebApp.Program.Main(Array.Empty<string>());

            Console.WriteLine("All started!");

            Console.ReadKey();
        }

        public static void StartAllManagers()
        {
            Console.WriteLine("Starting TimeManager...");
            TimeManager.Start();
            Console.WriteLine("Started!");
        }
    }
}
