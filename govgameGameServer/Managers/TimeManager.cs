using System;
using System.Collections.Generic;
using System.Timers;

namespace govgameGameServer.Managers
{
    public static class TimeManager
    {
        private static int irlSecondsInGameSecond = 5; // this is essentialy a multiplier. The game time is irlSecondsInGameSecond times faster than real life
        private static int gameTimerInterval = (int)((1 / (decimal)irlSecondsInGameSecond) * 60 * 1000); // every interval, a game minute passes
        private static Timer GameTimer { get; set; }
        private static List<Action> GameTimerTickMethods = new List<Action>();

        public static int MinutesPastEpoch { get; private set; }

        public static void Start()
        {
            MinutesPastEpoch = 0;

            GameTimer = new Timer(gameTimerInterval);
            GameTimer.Start();
            Console.WriteLine("GameTimer started.");
            GameTimer.Elapsed += GameTimerIntervalElapsed;
        }

        private static void GameTimerIntervalElapsed(object source, ElapsedEventArgs elapsedEventArgs)
        {
            MinutesPastEpoch++;

            Console.WriteLine($"Game timer ticked. {MinutesPastEpoch} total minutes elapsed. Sending to clients.");

            foreach (Action action in GameTimerTickMethods)
            {
                action();
            }
        }

        public static void AddGameTimerTickMethod(Action method)
        {
            GameTimerTickMethods.Add(method);
        }
    }
}
