﻿using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using govgameSharedClasses.Helpers;

namespace govgameWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;

                return Redirect("/Game/Index");
            }
            else
            {
                if (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true))
                {
                    return Redirect("/Auth/CheckVerificationEmail");
                }

                return View();
            }
        }

        public IActionResult PrivacyPolicy()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie, true);

            ViewData["userLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult Error500()
        {
            return Content("Error: Internal server error.");
        }
    }
}
