using System;
using System.Linq;
using FirebaseAdmin.Auth;
using govgameSharedClasses.Helpers;
using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace govgameWebApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult LogIn(string redirect = "/")
        {
            string authSessionCookie = Request.Cookies["authSession"];

            switch (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie))
            {
                case true:
                    return Redirect(redirect);

                case false:
                    if (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true))
                    {
                        return Redirect("/Auth/CheckVerificationEmail");
                    }

                    ViewData["redirectPath"] = redirect;

                    return View();
            }
        }

        public IActionResult Register()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            switch (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true))
            {
                case true:
                    return Redirect("/");

                case false:
                    return View();
            }
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("authSession");
            return Redirect("/");
        }

        public IActionResult CheckVerificationEmail()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            if (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie))
            {
                return Redirect("/");
            }

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["userLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult EmailVerified()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["userLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult NextSteps()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["userLoggedIn"] = userLoggedIn;

            return View();
        }

        [HttpPost]
        public IActionResult LogInPOST()
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            string idTokenOrError = FirebaseAuthHelper.SignInWithEmailAndPassword(email, password);
            string idToken;
            if (idTokenOrError.StartsWith("error: "))
            {
                JObject errorJObject = JObject.Parse(idTokenOrError.Remove(0, 7));
                return Content(FirebaseAuthHelper.GenerateNiceErrorMessage(errorJObject["message"].ToString()));
            }
            else
            {
                idToken = idTokenOrError.Remove(0, 9);
            }

            TimeSpan expirationTimeSpan;
            if (Request.Form["remember-me"] == "on")
            {
                expirationTimeSpan = TimeSpan.FromDays(14);
            }
            else
            {
                expirationTimeSpan = TimeSpan.FromDays(1);
            }

            SessionCookieOptions options = new SessionCookieOptions()
            {
                ExpiresIn = expirationTimeSpan
            };

            try
            {
                string sessionCookie = FirebaseAuth.DefaultInstance.CreateSessionCookieAsync(idToken, options).Result;

                CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.Add(options.ExpiresIn),
                    HttpOnly = true,
                    Secure = EnvironmentHelper.KeepCookiesSecure()
                };

                Response.Cookies.Append("authSession", sessionCookie, cookieOptions);
                return Content("success");
            }
            catch (FirebaseAuthException)
            {
                return Content("Error: Failed to create a session cookie.");
            }
        }

        [HttpPost]
        public IActionResult RegisterPOST()
        {
            string email = Request.Form["email"];
            string password = Request.Form["password"];

            UserRecordArgs userRecordArgs = new UserRecordArgs
            {
                Email = email,
                EmailVerified = false,
                Password = password,
                Disabled = false
            };

            UserRecord userRecord;
            try
            {
                userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs).Result;
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    return Content("Invalid email or password. Your password may be too weak.");
                }

                if (e is FirebaseAuthException firebaseAuthException)
                {
                    return Content(FirebaseAuthHelper.GenerateNiceErrorMessage(firebaseAuthException.AuthErrorCode));
                }

                if (e is AggregateException aggregateException)
                {
                    foreach (Exception exception in aggregateException.InnerExceptions)
                    {
                        if (exception is ArgumentException)
                        {
                            return Content("Invalid email or password. Your password may be too weak.");
                        }

                        if (exception is FirebaseAuthException aggregateFirebaseAuthException)
                        {
                            return Content(FirebaseAuthHelper.GenerateNiceErrorMessage(aggregateFirebaseAuthException.AuthErrorCode));
                        }
                    }
                }

                return Content("Error: Unknown error occurred.");
            }

            string idTokenOrError = FirebaseAuthHelper.SignInWithEmailAndPassword(email, password);
            string idToken;
            if (idTokenOrError.StartsWith("error: "))
            {
                JObject errorJObject = JObject.Parse(idTokenOrError.Remove(0, 7));
                return Content(FirebaseAuthHelper.GenerateNiceErrorMessage(errorJObject["message"].ToString()));
            }
            else
            {
                idToken = idTokenOrError.Remove(0, 9);
            }

            using (DatabaseContext database = new DatabaseContext())
            {
                string username = Request.Form["username"];

                if (database.Users.Any(u => u.Username == username))
                {
                    FirebaseAuth.DefaultInstance.DeleteUserAsync(userRecord.Uid).Wait();
                    return Content("There is another user with that username, and we don't allow duplicate usernames. Sorry!");
                }

                User user = new User
                {
                    Username = username,
                    FirebaseUid = userRecord.Uid
                };
                database.Users.Add(user);
                database.SaveChanges();
            }

            FirebaseAuthHelper.SendVerificationEmail(idToken);

            TimeSpan expirationTimeSpan;
            if (Request.Form["remember-me"] == "on")
            {
                expirationTimeSpan = TimeSpan.FromDays(14);
            }
            else
            {
                expirationTimeSpan = TimeSpan.FromDays(1);
            }

            SessionCookieOptions options = new SessionCookieOptions()
            {
                ExpiresIn = expirationTimeSpan
            };

            try
            {
                string sessionCookie = FirebaseAuth.DefaultInstance.CreateSessionCookieAsync(idToken, options).Result;

                CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.Add(options.ExpiresIn),
                    HttpOnly = true,
                    Secure = EnvironmentHelper.KeepCookiesSecure()
                };

                Response.Cookies.Append("authSession", sessionCookie, cookieOptions);
                return Content("success");
            }
            catch (FirebaseAuthException)
            {
                return Content("Error: Failed to create a session cookie.");
            }
        }
    }
}
