using System;
using govgameGameServer.Managers;

namespace govgameGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            throw new NotImplementedException();
        }

        public static void StartAllManagers()
        {
            Console.WriteLine("Starting TimeManager...");
            TimeManager.Start();
            Console.WriteLine("Started!");
        }
    }
}
