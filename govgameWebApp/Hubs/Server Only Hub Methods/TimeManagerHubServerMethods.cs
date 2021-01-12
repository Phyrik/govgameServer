using govgameGameServer.Managers;
using govgameWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace govgameWebApp.Hubs.Server_Only_Hub_Methods
{
    public class TimeManagerHubServerMethods
    {
        public static IHubContext<TimeManagerHub> TimeManagerHubContext { get; set; }

        public static void BroadcastNewTime()
        {
            TimeManagerHubContext.Clients.All.SendAsync("NewTime", TimeManager.MinutesPastEpoch).Wait();
        }

        public static void BroadcastNewTime(object source, ElapsedEventArgs elapsedEventArgs)
        {
            TimeManagerHubContext.Clients.All.SendAsync("NewTime", TimeManager.MinutesPastEpoch).Wait();
        }
    }
}
