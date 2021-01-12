using FirebaseAdmin.Auth;

namespace govgameWebApp.Helpers
{
    public class FirebaseAuthHelper
    {
        public static bool IsUserLoggedIn(string authSessionCookie)
        {
            if (string.IsNullOrEmpty(authSessionCookie))
            {
                return false;
            }

            try
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie, true).Result;

                UserRecord firebaseUser = FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken.Uid).Result;

                if (firebaseUser.EmailVerified)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
