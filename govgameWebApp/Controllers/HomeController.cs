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

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseToken.Uid);

                if (!publicUser.OwnsCountry && !publicUser.IsMinister)
                {
                    return Redirect("/Auth/NextSteps");
                }
                else
                {
                    return Redirect("/Game/Index");
                }
            }
            else
            {
                return View();
            }
        }
    }
}
