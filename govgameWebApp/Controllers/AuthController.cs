﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            switch (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie))
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

            switch (FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie))
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

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            ViewData["useLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult EmailVerified()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            ViewData["useLoggedIn"] = userLoggedIn;

            return View();
        }

        public IActionResult NextSteps()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

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

            if (idToken == null)
            {
                return Unauthorized("No id token provided.");
            }

            AuthHelper.CreateInitialUserInfoDocument(FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken).Result, username);

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
                return Redirect("/Auth/CheckVerificationEmail");
            }
            catch (FirebaseAuthException)
            {
                return Unauthorized("Failed to create a session cookie");
            }
        }
    }
}