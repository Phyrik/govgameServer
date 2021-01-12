using System;
using System.Timers;

namespace govgameGameServer.Managers
{
    public static class TimeManager
    {
        private static int irlSecondsInGameSecond = 5; // this is essentialy a multiplier. The game time is irlSecondsInGameSecond times faster than real life
        private static int gameTimerInterval = (int)((1 / (decimal)irlSecondsInGameSecond) * 60 * 1000); // every interval, a game minute passes
        public static Timer GameTimer { get; private set; }

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
        }
    }
}
