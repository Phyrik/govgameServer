using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using govgameSharedClasses.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;

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

            Directory.SetCurrentDirectory("govgameWebApp");
            Console.WriteLine("govgameWebApp current working directory: " + Directory.GetCurrentDirectory());

            IHost govgameWebAppIHost = CreateHostBuilder(args).Build();
            govgameWebAppIHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, EnvironmentHelper.PortToRunOn(), listenOptions =>
                        {
                            listenOptions.UseHttps("finalcert.pfx", "friends2021");
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
