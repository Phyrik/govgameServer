using FirebaseAdmin.Auth;
using govgameSharedClasses.Models.MongoDB;
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
                return View();
            }
        }

        public IActionResult PrivacyPolicy()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            ViewData["userLoggedIn"] = userLoggedIn;

            return View();
        }
    }
}
