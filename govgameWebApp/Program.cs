using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using govgameGameServer.Managers;
using govgameWebApp.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Timers;

namespace govgameWebApp
{
    public class Program
    {
        private static IHubContext<TimeManagerHub> TimeManagerHubContext { get; set; }

        public static void Main(string[] args)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("government-game-firebase-adminsdk-8gpmw-d6a3303ab7.json")
            });

            IHost govgameWebAppIHost = CreateHostBuilder(args).Build();
            TimeManagerHubContext = (IHubContext<TimeManagerHub>)govgameWebAppIHost.Services.GetService(typeof(IHubContext<TimeManagerHub>));
            govgameWebAppIHost.RunAsync();

            govgameGameServer.Program.StartAllManagers();
            TimeManager.GameTimer.Elapsed += BroadcastNewTime;

            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void BroadcastNewTime(object source, ElapsedEventArgs elapsedEventArgs)
        {
            Console.WriteLine("Sending clients new time...");
            TimeManagerHubContext.Clients.All.SendAsync("NewTime", TimeManager.MinutesPastEpoch).Wait();
            Console.WriteLine("Done!");
        }
    }
}
