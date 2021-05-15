using System.IO;

namespace govgameSharedClasses.Helpers
{
    public class PrivateKeyAndPasswordsHelper
    {
        public static string GetMySQLPassword()
        {
            return File.ReadAllText("/home/pi/Documents/private_keys/mariadbpassword.txt").Trim();
        }

        public static string GetFirebasePrivateKeyPath()
        {
            return "/home/pi/Documents/private_keys/government-game-firebase-adminsdk-8gpmw-648b7e8b70.json";
        }

        public static string GetSSLCertificatePath()
        {
            return "/home/pi/Documents/private_keys/govgame.pfx";
        }

        public static string GetSSLCertificatePassword()
        {
            return File.ReadAllText("/home/pi/Documents/private_keys/sslcertpassword.txt").Trim();
        }

        public static string GetFirebaseAPIKey()
        {
            return File.ReadAllText("/home/pi/Documents/private_keys/firebaseapikey.txt").Trim();
        }
    }
}
