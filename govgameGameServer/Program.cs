using System;
using govgameGameServer.Managers;

namespace govgameGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("govgameGameServer initialising...");

            StartAllManagers(args);

            Console.ReadKey();
        }

        private static void StartAllManagers(string[] apiAppArgs)
        {
            Console.WriteLine("Starting TimeManager...");
            TimeManager.Start();
            Console.WriteLine("Started!");
        }
    }
}
