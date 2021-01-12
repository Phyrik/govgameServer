using FirebaseAdmin.Auth;
using govgameWebApp.Helpers;
using govgameWebApp.Models.MongoDB;
using govgameWebApp.Models.OtherModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace govgameWebApp.Controllers
{
    public class GameActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string authSessionCookie = context.HttpContext.Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                Controller controller = (Controller)context.Controller;

                context.ActionArguments.Add("authSessionCookie", authSessionCookie);

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                controller.ViewData["publicUser"] = publicUser;

                Country country = MongoDBHelper.GetCountry(publicUser.CountryId);

                controller.ViewData["country"] = country;

                int unreadEmails = 0;
                foreach (Email email in MongoDBHelper.GetUsersReceivedEmails(firebaseUid))
                {
                    if (!email.MarkedAsRead)
                    {
                        unreadEmails++;
                    }
                }

                controller.ViewData["unreadEmails"] = unreadEmails;
                
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new RedirectResult("/");
            }
        }
    }

    [GameActionFilter]
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Emails(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            switch (page)
            {
                case null:
                    Email[] emails = MongoDBHelper.GetUsersReceivedEmails(firebaseUid);
                    ViewData["emails"] = emails;

                    return View("./Emails/Index");

                case "NewEmail":
                    PublicUser[] allPublicUsers = MongoDBHelper.GetAllPublicUsers();
                    ViewData["allPublicUsers"] = allPublicUsers;

                    return View("./Emails/New");

                default:
                    return Content("Error 404: Whoops...you shouldn't be here.");
            }
        }

        public IActionResult CountryDashboard(string page, string authSessionCookie)
        {
            switch (page)
            {
                case null:
                    return View();

                default:
                    return Content("Error 404: Whoops...you shouldn't be here.");
            }
        }

        public IActionResult PrimeMinisterDashboard(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

            ViewData["publicUser"] = publicUser;

            Country country = MongoDBHelper.GetCountry(publicUser.CountryId);

            ViewData["country"] = country;

            if (country.PrimeMinisterId != publicUser.UserId)
            {
                return Content("error: you are not authorised to see this page");
            }

            switch (page)
            {
                case null:
                    return View("./PrimeMinisterDashboard/Index");

                case "MinistryManagement":
                    ViewData["ministries"] = Ministry.GetAllMinistries(publicUser.CountryId);
                    return View("./PrimeMinisterDashboard/MinistryManagement");

                case "InviteNewMinister":
                    Ministry.MinistryCode ministryCode = (Ministry.MinistryCode)Enum.Parse(typeof(Ministry.MinistryCode), Request.Query["minister"]);
                    ViewData["ministryCode"] = ministryCode;

                    PublicUser[] allPublicUsers = MongoDBHelper.GetAllPublicUsers();
                    ViewData["allPublicUsers"] = allPublicUsers;

                    return View("./PrimeMinisterDashboard/InviteNewMinister");

                default:
                    return Content("Error 404: Whoops...you shouldn't be here.");
            }
        }

        #region POST Requests
        [HttpPost]
        public IActionResult DismissMinister(string minister)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                Country country = MongoDBHelper.GetCountry(publicUser.CountryId);

                if (country.PrimeMinisterId == publicUser.UserId)
                {
                    Ministry.MinistryCode ministryCode = (Ministry.MinistryCode)Enum.Parse(typeof(Ministry.MinistryCode), minister);

                    CountryUpdate countryUpdate;
                    switch (ministryCode)
                    {
                        case Ministry.MinistryCode.Interior:
                            countryUpdate = new CountryUpdate { InteriorMinisterId = "none" };

                            if (MongoDBHelper.UpdateCountry(country.CountryId, countryUpdate))
                            {
                                return Content("success");
                            }
                            else
                            {
                                return Content("error: internal server error");
                            }

                        case Ministry.MinistryCode.FinanceAndTrade:
                            countryUpdate = new CountryUpdate { FinanceAndTradeMinisterId = "none" };

                            if (MongoDBHelper.UpdateCountry(country.CountryId, countryUpdate))
                            {
                                return Content("success");
                            }
                            else
                            {
                                return Content("error: internal server error");
                            }

                        case Ministry.MinistryCode.ForeignAffairs:
                            countryUpdate = new CountryUpdate { ForeignMinisterId = "none" };

                            if (MongoDBHelper.UpdateCountry(country.CountryId, countryUpdate))
                            {
                                return Content("success");
                            }
                            else
                            {
                                return Content("error: internal server error");
                            }

                        case Ministry.MinistryCode.Defence:
                            countryUpdate = new CountryUpdate { DefenceMinisterId = "none" };

                            if (MongoDBHelper.UpdateCountry(country.CountryId, countryUpdate))
                            {
                                return Content("success");
                            }
                            else
                            {
                                return Content("error: internal server error");
                            }

                        default:
                            return Content("error: invalid minister");
                    }
                }
                else
                {
                    return StatusCode(401);
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }

        [HttpPost]
        public IActionResult SendEmail()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                string[] toUserIds = Request.Form["to"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                string emailSubject = Request.Form["subject"];
                string emailBody = Request.Form["body"];

                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                bool sendEmailSuccess = MongoDBHelper.SendEmail(new EmailSendRequest
                {
                    SenderId = publicUser.UserId,
                    RecipientIds = toUserIds,
                    Subject = emailSubject,
                    Body = emailBody
                });

                if (sendEmailSuccess)
                {
                    return View("CloseWindow");
                }
                else
                {
                    return Content("error: internal server error");
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }

        [HttpPost]
        public IActionResult MarkEmailAsReadUnread(string emailId, string readOrUnread)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                Email emailToMark = MongoDBHelper.GetEmailById(emailId);

                if (emailToMark.RecipientId != firebaseUid)
                {
                    return StatusCode(401);
                }

                switch (readOrUnread)
                {
                    case "read":
                        bool markEmailAsReadSuccess = MongoDBHelper.MarkEmailAsRead(emailId);

                        if (markEmailAsReadSuccess)
                        {
                            return Content("success");
                        }
                        else
                        {
                            return Content("error: internal server error");
                        }

                    case "unread":
                        bool markEmailAsUnreadSuccess = MongoDBHelper.MarkEmailAsUnread(emailId);

                        if (markEmailAsUnreadSuccess)
                        {
                            return Content("success");
                        }
                        else
                        {
                            return Content("error: internal server error");
                        }

                    default:
                        return Content("invalid or missing parameter: readOrUnread");
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }
        #endregion

        #region CreateACountry
        public IActionResult CreateACountry(string authSessionCookie)
        {
            ViewData["userLoggedIn"] = true;

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

            if (!publicUser.OwnsCountry && !publicUser.IsMinister)
            {
                return View();
            }
            else
            {
                return Redirect("/Game/Index");
            }
        }

        [HttpPost]
        public IActionResult CreateACountryPOST()
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.GetPublicUser(firebaseUid);

                if (!publicUser.OwnsCountry && !publicUser.IsMinister)
                {
                    Country country = new Country
                    {
                        CountryId = CountryGenerationHelper.GenerateCountryUUID(),
                        CountryName = Request.Form["country-name"],
                        Demonym = Request.Form["demonym"],
                        CapitalName = Request.Form["capital-name"],
                        FlagId = CountryGenerationHelper.FlagNameToId(Request.Form["flag-name"]),
                        PrimeMinisterId = firebaseUid,
                        InteriorMinisterId = "none",
                        FinanceAndTradeMinisterId = "none",
                        ForeignMinisterId = "none",
                        DefenceMinisterId = "none"
                    };

                    GlobalLocationIdentifier globalLocationIdentifier = new GlobalLocationIdentifier(int.Parse(Request.Form["locationX"]) - 50, int.Parse(Request.Form["locationY"]) - 50);

                    Location[] locations = MongoDBHelper.GetLocations(globalLocationIdentifier, 100, 100);
                    foreach (Location location in locations)
                    {
                        if (location.Owner != "none")
                        {
                            return Content("error: locations already owned by another country");
                        }
                    }

                    LocationUpdate locationUpdate = new LocationUpdate
                    {
                        Owner = country.CountryId
                    };
                    if (MongoDBHelper.UpdateLocations(globalLocationIdentifier, 100, 100, locationUpdate))
                    {
                        MongoDBHelper.NewCountry(country);

                        MongoDBHelper.UpdateUser(firebaseUid, new UserUpdate
                        {
                            OwnsCountry = true,
                            IsMinister = true,
                            CountryId = country.CountryId
                        });

                        return Redirect("/");
                    }
                    else
                    {
                        return Content("error: could not request locations");
                    }
                }
                else
                {
                    return Content("error: you already own a country or are a minister in another");
                }
            }
            else
            {
                return Redirect("error: you are not logged in");
            }
        }
        #endregion
    }
}
