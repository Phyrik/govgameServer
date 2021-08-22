using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using govgameSharedClasses.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using ElectronNET.API;

namespace govgameWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(PrivateKeyAndPasswordsHelper.GetFirebasePrivateKeyPath())
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
                    webBuilder.UseElectron(args);
                    webBuilder.UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, EnvironmentHelper.PortToRunOn(), listenOptions =>
                        {
                            listenOptions.UseHttps(PrivateKeyAndPasswordsHelper.GetSSLCertificatePath(), PrivateKeyAndPasswordsHelper.GetSSLCertificatePassword());
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}

// TODO: GLOBAL: change all hrefs in a tags to asp-controller and asp-action