using System;
using System.Timers;

namespace govgameGameServer.Managers
{
    public static class TimeManager
    {
        private static int irlSecondsInGameSecond = 5; // this is essentialy a multiplier. The game time is irlSecondsInGameSecond times faster than real life
        private static int gameTimerInterval = 1; // every gameTimeInterval seconds, the game updates the game time
        public static Timer GameTimer { get; private set; }

        public static DateTime Epoch { get; private set; }
        public static int MinutesPastEpoch { get; private set; }

        public static void Start()
        {
            Epoch = new DateTime(2021, 1, 13, 0, 0, 0, DateTimeKind.Utc);

            GameTimer = new Timer(gameTimerInterval * 1000);
            GameTimer.Start();
            Console.WriteLine("GameTimer started.");
            GameTimer.Elapsed += GameTimerIntervalElapsed;
        }

        private static void GameTimerIntervalElapsed(object source, ElapsedEventArgs elapsedEventArgs)
        {
            MinutesPastEpoch = (int)((DateTime.UtcNow - Epoch).TotalMinutes * irlSecondsInGameSecond);
        }
    }
}
