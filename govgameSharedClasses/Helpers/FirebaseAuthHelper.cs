using FirebaseAdmin.Auth;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace govgameSharedClasses.Helpers
{
    public class FirebaseAuthHelper
    {
        static string firebaseApiKey = PrivateKeyAndPasswordsHelper.GetFirebaseAPIKey();

        public static bool IsUserLoggedIn(string authSessionCookie, bool dontCheckEmailVerified = false)
        {
#if DEBUG
            dontCheckEmailVerified = true;
#endif

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

        /// <returns>Either "idToken: [USER'S ID TOKEN]" or "error: [ERROR JSON]"</returns>
        public static string SignInWithEmailAndPassword(string email, string password)
        {
            WebRequest request = WebRequest.Create($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}");
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes($"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}");
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            string response;
            try
            {
                response = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (WebException e)
            {
                response = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();
            }

            JObject responseJObject = JObject.Parse(response);

            if (responseJObject.ContainsKey("error"))
            {
                string errorJson = responseJObject["error"].ToString();
                return $"error: {errorJson}";
            }
            else
            {
                string idToken = responseJObject["idToken"].ToString();
                return $"idToken: {idToken}";
            }
        }

        /// <returns>Either "success" or "error: [ERROR JSON]"</returns>
        public static string SendVerificationEmail(string idToken)
        {
            WebRequest request = WebRequest.Create($"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={firebaseApiKey}");
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] data = Encoding.UTF8.GetBytes($"{{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"{idToken}\"}}");
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            string response = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

            JObject responseJObject = JObject.Parse(response);

            if (responseJObject.ContainsKey("error"))
            {
                string errorJson = responseJObject["error"].ToString();
                return $"error: {errorJson}";
            }
            else
            {
                return "success";
            }
        }

        public static string GenerateNiceErrorMessage(string firebaseMessage)
        {
            switch (firebaseMessage)
            {
                case "MISSING_EMAIL":
                    return "Please enter an email.";
                case "INVALID_EMAIL":
                    return "Invalid email.";
                case "MISSING_PASSWORD":
                    return "Please enter a password.";
                case "EMAIL_NOT_FOUND":
                    return "No user found with that email. Try registering first.";
                case "INVALID_PASSWORD":
                    return "Incorrect password.";
            }

            return firebaseMessage;
        }

        public static string GenerateNiceErrorMessage(AuthErrorCode? authErrorCode)
        {
            switch (authErrorCode)
            {
                case AuthErrorCode.EmailAlreadyExists:
                    return "Email already taken. Try logging in.";
            }

            return authErrorCode.ToString();
        }
    }
}