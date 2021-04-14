using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using govgameWebApp.Hubs;
using govgameWebApp.Hubs.Server_Only_Hub_Methods;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;

namespace govgameWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("government-game-firebase-adminsdk-8gpmw-d6a3303ab7.json")
            });

            Directory.SetCurrentDirectory("govgameWebapp");
            Console.WriteLine(Directory.GetCurrentDirectory());

            IHost govgameWebAppIHost = CreateHostBuilder(args).Build();
            TimeManagerHubServerMethods.TimeManagerHubContext = (IHubContext<TimeManagerHub>)govgameWebAppIHost.Services.GetService(typeof(IHubContext<TimeManagerHub>));
            govgameWebAppIHost.RunAsync();

            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel(options =>
                    //{
                    //    options.Listen(IPAddress.Any, 5000, listenOptions =>
                    //    {
                    //        listenOptions.UseHttps("certificate.pfx", "friends2021");
                    //    });
                    //});
                    webBuilder.UseStartup<Startup>();
                });
    }
}
