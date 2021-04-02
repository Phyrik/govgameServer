using System;
using System.Linq;
using FirebaseAdmin.Auth;
using govgameSharedClasses.Helpers;
using govgameSharedClasses.Models.MongoDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace govgameWebApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult LogIn(string redirect = "/")
        {
            string authSessionCookie = Request.Cookies["authSession"];

            switch (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true))
            {
                case true:
                    return Redirect(redirect);

                case false:
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
        public IActionResult LogInPOST(string redirect = "/")
        {
            string idToken = Request.Form["idToken"];

            if (idToken == null)
            {
                return Unauthorized("No id token provided.");
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
                    Secure = true
                };

                Response.Cookies.Append("authSession", sessionCookie, cookieOptions);
                return Redirect(redirect);
            }
            catch (FirebaseAuthException)
            {
                return Unauthorized("Failed to create a session cookie");
            }
        }

        [HttpPost]
        public IActionResult RegisterPOST()
        {
            string idToken = Request.Form["idToken"];
            string username = Request.Form["username"];

            if (idToken == null || idToken == string.Empty)
            {
                return Unauthorized("No id token provided.");
            }

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken).Result;

            string[] existingUsernames = MongoDBHelper.UsersDatabase.GetAllUsernames();
            if (existingUsernames.Contains(username))
            {
                FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseToken.Uid).Wait();
                return Content("error: username taken");
            }

            PublicUser publicUser = new PublicUser
            {
                UserId = firebaseToken.Uid,
                Username = username,
                CountryId = "none"
            };

            if (!MongoDBHelper.UsersDatabase.NewUser(publicUser))
            {
                FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseToken.Uid).Wait();
                return Content("error: internal server error");
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
                    Secure = true
                };

                Response.Cookies.Append("authSession", sessionCookie, cookieOptions);
                return Content("success");
            }
            catch (FirebaseAuthException)
            {
                return Unauthorized("Failed to create a session cookie");
            }
        }
    }
}
