using FirebaseAdmin.Auth;

namespace govgameWebApp.Helpers
{
    public class FirebaseAuthHelper
    {
        public static bool IsUserLoggedIn(string authSessionCookie, bool dontCheckEmailVerified = false)
        {
            if (string.IsNullOrEmpty(authSessionCookie))
            {
                return false;
            }

            try
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie, true).Result;

                UserRecord firebaseUser = FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken.Uid).Result;

                if (!dontCheckEmailVerified)
                {
                    if (firebaseUser.EmailVerified)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
