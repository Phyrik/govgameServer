using FirebaseAdmin.Auth;
using govgameSharedClasses.Models.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using govgameSharedClasses.Helpers;
using System.Linq;

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

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                controller.ViewData["publicUser"] = publicUser;

                try
                {
                    Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                    controller.ViewData["country"] = country;
                }
                catch
                {
                    controller.ViewData["country"] = null;
                }

                int unreadEmails = 0;
                foreach (Email email in MongoDBHelper.EmailsDatabase.GetUsersReceivedEmails(firebaseUid))
                {
                    if (!email.MarkedAsRead)
                    {
                        unreadEmails++;
                    }
                }
                int unreadNotifications = 0;
                foreach (Notification notification in MongoDBHelper.NotificationsDatabase.GetUsersReceivedNotifications(firebaseUid))
                {
                    if (!notification.MarkedAsRead)
                    {
                        unreadNotifications++;
                    }
                }

                controller.ViewData["unreadEmails"] = unreadEmails;
                controller.ViewData["unreadNotifications"] = unreadNotifications;

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

        public IActionResult Notifications(string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            Notification[] notifications = MongoDBHelper.NotificationsDatabase.GetUsersReceivedNotifications(firebaseUid);
            ViewData["notifications"] = notifications;

            return View();
        }

        public IActionResult Emails(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            switch (page)
            {
                case null:
                    Email[] emails = MongoDBHelper.EmailsDatabase.GetUsersReceivedEmails(firebaseUid);
                    ViewData["emails"] = emails;

                    return View("./Emails/Index");

                case "NewEmail":
                    PublicUser[] allPublicUsers = MongoDBHelper.UsersDatabase.GetAllPublicUsers();
                    ViewData["allPublicUsers"] = allPublicUsers;

                    return View("./Emails/New");

                default:
                    return Content("Error 404: Whoops...you shouldn't be here.");
            }
        }

        public IActionResult Invite(string page, string authSessionCookie, string countryId, string ministry)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            Country oldCountry = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

            switch (page)
            {
                case "Minister":
                    PublicUser oldPrimeMinisterUser = MongoDBHelper.UsersDatabase.GetPublicUser(oldCountry.PrimeMinisterId);
                    ViewData["oldPrimeMinisterUser"] = oldPrimeMinisterUser;

                    Country newCountry = MongoDBHelper.CountriesDatabase.GetCountry(countryId);
                    ViewData["newCountry"] = newCountry;

                    PublicUser newPrimeMinisterUser = MongoDBHelper.UsersDatabase.GetPublicUser(newCountry.PrimeMinisterId);
                    ViewData["newPrimeMinisterUser"] = newPrimeMinisterUser;

                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);
                    ViewData["ministryCode"] = ministryCode;

                    bool isPrimeMinister = publicUser.OwnsCountry;
                    ViewData["isPrimeMinister"] = isPrimeMinister;

                    if (newCountry.GetInvitedMinisterIdByCode(ministryCode) != publicUser.UserId)
                    {
                        return Content("error: invalid invite link");
                    }

                    return View("./Invite/Minister");

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

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            ViewData["publicUser"] = publicUser;

            Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

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
                    return View("./PrimeMinisterDashboard/MinistryManagement");

                case "InviteNewMinister":
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), Request.Query["minister"]);
                    ViewData["ministryCode"] = ministryCode;

                    PublicUser[] allPublicUsers = MongoDBHelper.UsersDatabase.GetAllPublicUsers();
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

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                if (country.PrimeMinisterId == publicUser.UserId)
                {
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), minister);

                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetMinisterIdByCode(ministryCode, "none");

                    UserUpdate userUpdate = new UserUpdate { IsMinister = false, Ministry = (int)MinistryHelper.MinistryCode.None, CountryId = "none" };

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(country.CountryId, countryUpdate) &&
                        MongoDBHelper.UsersDatabase.UpdateUser(country.GetMinisterIdByCode(ministryCode), userUpdate))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("error: internal server error");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }

        [HttpPost]
        public IActionResult InviteMinister(string ministry, string invitedUserId)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                if (country.PrimeMinisterId == publicUser.UserId)
                {
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetInvitedMinisterIdByCode(ministryCode, invitedUserId);

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(country.CountryId, countryUpdate))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("error: internal server error");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }

        [HttpPost]
        public IActionResult AcceptMinistryInvite(string ministry, string newCountryId, string ministryToReplacePM = null)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country newCountry = MongoDBHelper.CountriesDatabase.GetCountry(newCountryId);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (publicUser.UserId == newCountry.GetInvitedMinisterIdByCode(ministryCode))
                {
                    CountryUpdate newCountryUpdate = new CountryUpdate();
                    newCountryUpdate.SetMinisterIdByCode(ministryCode, publicUser.UserId);
                    newCountryUpdate.SetInvitedMinisterIdByCode(ministryCode, "none");

                    UserUpdate userUpdate = new UserUpdate { Ministry = (int)ministryCode, CountryId = newCountry.CountryId };

                    if (publicUser.IsMinister)
                    {
                        Country oldCountry = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                        CountryUpdate oldCountryUpdate = new CountryUpdate();
                        oldCountryUpdate.SetMinisterIdByCode((MinistryHelper.MinistryCode)publicUser.Ministry, "none");

                        if (publicUser.OwnsCountry)
                        {
                            MinistryHelper.MinistryCode ministryToReplacePMCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministryToReplacePM);

                            oldCountryUpdate.SetMinisterIdByCode(MinistryHelper.MinistryCode.PrimeMinister, oldCountry.GetMinisterIdByCode(ministryToReplacePMCode));
                            oldCountryUpdate.SetMinisterIdByCode(ministryToReplacePMCode, "none");
                            userUpdate.OwnsCountry = false;
                        }

                        if (!MongoDBHelper.CountriesDatabase.UpdateCountry(oldCountry.CountryId, oldCountryUpdate))
                        {
                            return Content("error: internal server error");
                        }
                    }
                    else
                    {
                        userUpdate.IsMinister = true;
                    }

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(newCountry.CountryId, newCountryUpdate) &&
                        MongoDBHelper.UsersDatabase.UpdateUser(publicUser.UserId, userUpdate))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("error: internal server error");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("error: not logged in");
            }
        }

        [HttpPost]
        public IActionResult DeclineMinistryInvite(string ministry, string newCountryId)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country newCountry = MongoDBHelper.CountriesDatabase.GetCountry(newCountryId);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (publicUser.UserId == newCountry.GetInvitedMinisterIdByCode(ministryCode))
                {
                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetInvitedMinisterIdByCode(ministryCode, "none");

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(newCountryId, countryUpdate))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("error: internal server error");
                    }
                }
                else
                {
                    return StatusCode(403);
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

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                bool sendEmailSuccess = MongoDBHelper.EmailsDatabase.SendEmail(new EmailSendRequest
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

                Email emailToMark = MongoDBHelper.EmailsDatabase.GetEmailById(emailId);

                if (emailToMark.RecipientId != firebaseUid)
                {
                    return StatusCode(403);
                }

                switch (readOrUnread)
                {
                    case "read":
                        bool markEmailAsReadSuccess = MongoDBHelper.EmailsDatabase.MarkEmailAsRead(emailId);

                        if (markEmailAsReadSuccess)
                        {
                            return Content("success");
                        }
                        else
                        {
                            return Content("error: internal server error");
                        }

                    case "unread":
                        bool markEmailAsUnreadSuccess = MongoDBHelper.EmailsDatabase.MarkEmailAsUnread(emailId);

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

        [HttpPost]
        public IActionResult MarkNotificationAsRead(string notificationId)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                Notification notificationToMark = MongoDBHelper.NotificationsDatabase.GetNotificationById(notificationId);

                if (notificationToMark.UserId != firebaseUid)
                {
                    return StatusCode(403);
                }

                bool markNotificationAsReadSuccess = MongoDBHelper.NotificationsDatabase.MarkNotificationAsRead(notificationId);

                if (markNotificationAsReadSuccess)
                {
                    return Content("success");
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
        #endregion

        #region CreateACountry
        public IActionResult CreateACountry(string authSessionCookie)
        {
            ViewData["userLoggedIn"] = true;

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

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

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                if (!publicUser.OwnsCountry && !publicUser.IsMinister)
                {
                    Country country = new Country
                    {
                        CountryId = CountryGenerationHelper.GenerateCountryUUID(),
                        CountryName = Request.Form["country-name"],
                        CapitalName = Request.Form["capital-name"],
                        FlagId = CountryGenerationHelper.FlagNameToId(Request.Form["flag-name"]),
                        PrimeMinisterId = firebaseUid,
                        InteriorMinisterId = "none",
                        FinanceAndTradeMinisterId = "none",
                        ForeignMinisterId = "none",
                        DefenceMinisterId = "none"
                    };

                    string[] existingCountryNames = MongoDBHelper.CountriesDatabase.GetAllCountryNames();
                    if (existingCountryNames.Contains(country.CountryName))
                    {
                        return Content("error: country name taken");
                    }

                    GlobalLocationIdentifier globalLocationIdentifier = new GlobalLocationIdentifier(int.Parse(Request.Form["locationX"]) - 50, int.Parse(Request.Form["locationY"]) - 50);

                    Location[] locations = MongoDBHelper.MapDatabase.GetLocations(globalLocationIdentifier, 100, 100);
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
                    if (MongoDBHelper.MapDatabase.UpdateLocations(globalLocationIdentifier, 100, 100, locationUpdate))
                    {
                        MongoDBHelper.CountriesDatabase.NewCountry(country);

                        MongoDBHelper.UsersDatabase.UpdateUser(firebaseUid, new UserUpdate
                        {
                            OwnsCountry = true,
                            IsMinister = true,
                            Ministry = (int)MinistryHelper.MinistryCode.PrimeMinister,
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
