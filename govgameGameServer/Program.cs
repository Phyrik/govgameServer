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
            Console.WriteLine("Started!");

            Console.WriteLine("Starting the govgameWebApp");
            govgameWebApp.Program.Main(args);
            Console.WriteLine("Started!");

            Console.WriteLine("All projects started!");

            Console.ReadKey();
        }

        public static void StartAllManagers()
        {
            Console.WriteLine("Starting TimeManager...");
            TimeManager.Start();
            Console.WriteLine("Started manager!");
        }
    }
}
