using System;
using System.Linq;
using FirebaseAdmin.Auth;
using govgameWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace govgameWebApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult LogIn()
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

        public IActionResult SignOut()
        {
            Response.Cookies.Delete("authSession");
            return Redirect("/");
        }

        public IActionResult CheckVerificationEmail()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["useLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult EmailVerified()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["useLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult NextSteps()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["useLoggedIn"] = userLoggedIn;

            return View();
        }

        [HttpPost]
        public IActionResult LogInPOST()
        {
            string idToken = Request.Form["idToken"];

            if (idToken == null)
            {
                return Unauthorized("No id token provided.");
            }

            SessionCookieOptions options = new SessionCookieOptions()
            {
                ExpiresIn = TimeSpan.FromDays(5)
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
                return Redirect("/");
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

            string[] existingUsernames = MongoDBHelper.GetAllUsernames();
            if (existingUsernames.Contains(username))
            {
                FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseToken.Uid).Wait();
                return Content("error: username taken");
            }

            AuthHelper.CreateInitialUserInfoDocument(firebaseToken, username);

            SessionCookieOptions options = new SessionCookieOptions()
            {
                ExpiresIn = TimeSpan.FromDays(5)
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
