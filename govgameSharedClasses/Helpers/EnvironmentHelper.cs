using System;

namespace govgameSharedClasses.Helpers
{
    public class EnvironmentHelper
    {
        public static bool KeepCookiesSecure()
        {
            switch (Environment.GetCommandLineArgs()[1])
            {
                case "secure":
                    return true;

                case "insecure":
                    return false;

                default:
                    throw new Exception("You must include a command line argument for cookie security. Try running \"dotnet run {secure/insecure} --project govgameGameServer\".");
            }
        }
    }
}
