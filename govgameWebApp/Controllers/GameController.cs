using FirebaseAdmin.Auth;
using govgameSharedClasses.Helpers;
using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.IO;
using System.Linq;
using System.Web;

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

                using (DatabaseContext database = new DatabaseContext())
                {
                    Controller controller = (Controller)context.Controller;

                    controller.ViewData["userLoggedIn"] = userLoggedIn;

                    context.ActionArguments.Add("authSessionCookie", authSessionCookie);

                    User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                    controller.ViewData["user"] = user;

                    try
                    {
                        Country country = database.Countries.Single(c => c.CountryName == user.CountryName);

                        controller.ViewData["country"] = country;
                    }
                    catch
                    {
                        controller.ViewData["country"] = null;
                    }

                    controller.ViewData["ministryDashboard"] = MinistryHelper.MinistryCode.None;

                    UserEmail[] userEmails = database.UserEmails.Where(ue => ue.ReceivingUsername == user.Username).ToArray();
                    int unreadEmails = userEmails.Where(ue => !ue.MarkedAsRead).Count();

                    Notification[] notifications = database.Notifications.Where(n => n.Username == user.Username).ToArray();
                    int unreadNotifications = notifications.Where(n => !n.MarkedAsRead).Count();

                    controller.ViewData["unreadEmails"] = unreadEmails;
                    controller.ViewData["unreadNotifications"] = unreadNotifications;

                    controller.ViewData["noCountry"] = user.CountryName == null;

                    base.OnActionExecuting(context);
                }
            }
            else
            {
                Uri redirectUri = new Uri(context.HttpContext.Request.GetDisplayUrl());

                string redirectPath = HttpUtility.UrlEncode(redirectUri.PathAndQuery);

                context.Result = new RedirectResult($"/Auth/LogIn?redirect={redirectPath}");
            }
        }
    }

    [GameActionFilter]
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/Game/Home");
        }

        public IActionResult Home(string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                switch (user.CountryName != null)
                {
                    case true:
                        return View();

                    case false:
                        return View("./NoCountry/Home");
                }
            }
        }

        public IActionResult Notifications(string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Notification[] notifications = database.Notifications.Where(n => n.Username == user.Username).ToArray();
                ViewData["notifications"] = notifications;

                return View();
            }
        }

        //public IActionResult Emails(string page, string authSessionCookie)
        //{
        //    FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
        //    string firebaseUid = firebaseToken.Uid;

        //    using (DatabaseContext database = new DatabaseContext())
        //    {
        //        User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

        //        switch (page)
        //        {
        //            case null:
        //                UserEmail[] userEmails = database.UserEmails.Where(ue => ue.ReceivingUsername == user.Username).ToArray();
        //                ViewData["userEmails"] = userEmails;

        //                return View("./Emails/Index");

        //            case "NewEmail":
        //                User[] allUsers = database.Users.ToArray();
        //                ViewData["allUsers"] = allUsers;

        //                return View("./Emails/New");

        //            default:
        //                return View("404");
        //        }
        //    }
        //}

        public IActionResult Invite(string page, string authSessionCookie, string countryName, string ministry)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country oldCountry;
                try
                {
                    oldCountry = database.Countries.Single(c => c.CountryName == user.CountryName);
                }
                catch
                {
                    oldCountry = null;
                }

                switch (page)
                {
                    case "Minister":
                        try
                        {
                            User oldPrimeMinister = database.Users.Single(u => u.CountryName == oldCountry.CountryName && u.Ministry == MinistryHelper.MinistryCode.PrimeMinister);
                            ViewData["oldPrimeMinister"] = oldPrimeMinister;
                        }
                        catch
                        {
                            ViewData["oldPrimeMinister"] = null;
                        }

                        Country newCountry = database.Countries.Single(c => c.CountryName == countryName);
                        ViewData["newCountry"] = newCountry;

                        User newPrimeMinisterUser = database.Users.Single(u => u.CountryName == newCountry.CountryName && u.Ministry == MinistryHelper.MinistryCode.PrimeMinister);
                        ViewData["newPrimeMinister"] = newPrimeMinisterUser;

                        MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);
                        ViewData["ministryCode"] = ministryCode;

                        bool isPrimeMinister = user.Ministry == MinistryHelper.MinistryCode.PrimeMinister;
                        ViewData["isPrimeMinister"] = isPrimeMinister;

                        if (isPrimeMinister)
                        {
                            bool noMinistersToReplace = true;
                            if (database.Users.Any(u => u.CountryName == oldCountry.CountryName && u.Ministry != MinistryHelper.MinistryCode.PrimeMinister)) noMinistersToReplace = false;
                            ViewData["noMinistersToReplace"] = noMinistersToReplace;
                        }
                        else
                        {
                            ViewData["noMinistersToReplace"] = null;
                        }

                        if (database.InvitedMinisters.Any(im => im.Username == user.Username && im.CountryName == newCountry.CountryName && im.Ministry == ministryCode))
                        {
                            ViewData["errorMessage"] = "Invalid invite link. The person who invited you may have cancelled the invitation.";
                            return View("../Error/TextError");
                        }

                        return View("./Invite/Minister");

                    default:
                        return View("404");
                }
            }
        }

        public IActionResult CountryDashboard(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                if (user.CountryName == null)
                {
                    return Redirect("/Game/Index");
                }

                switch (page)
                {
                    case null:
                        return View();

                    default:
                        return View("404");
                }
            }
        }

        public IActionResult PrimeMinisterDashboard(string authSessionCookie, string page = "Index")
        {
            ViewData["ministryDashboard"] = MinistryHelper.MinistryCode.PrimeMinister;

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                if (user.CountryName == null)
                {
                    return Redirect("/Game/Index");
                }

                if (!MinistryHelper.CanUserAccessMinistryDashboard(user.Username, user.CountryName, MinistryHelper.MinistryCode.PrimeMinister))
                {
                    return View("403");
                }

                MinistryHelper.MinistryCode ministryCode;
                switch (page)
                {
                    case "InviteNewMinister":
                        ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), Request.Query["minister"]);
                        ViewData["ministryCode"] = ministryCode;

                        User[] allUsers = database.Users.ToArray();
                        ViewData["allUsers"] = allUsers;

                        break;

                    case "ViewMinistry":
                        ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), Request.Query["ministry"]);
                        ViewData["ministryCode"] = ministryCode;

                        break;

                    default:
                        break;
                }

                return ViewHelper.GetMinistryDashboardView(this, Directory.GetCurrentDirectory(), MinistryHelper.MinistryCode.PrimeMinister, page);
            }
        }

        #region POST Requests
        [HttpPost]
        public IActionResult DismissMinister(string authSessionCookie, string minister)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country country = database.Countries.Single(c => c.CountryName == user.CountryName);

                if (user.Ministry == MinistryHelper.MinistryCode.PrimeMinister)
                {
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), minister);

                    if (!database.Users.Any(u => u.CountryName == country.CountryName && u.Ministry == ministryCode))
                    {
                        return Content("Error: There is no minister to dismiss!");
                    }

                    User ministerToDismiss = database.Users.Single(u => u.CountryName == country.CountryName && u.Ministry == ministryCode);
                    ministerToDismiss.CountryName = null;
                    ministerToDismiss.Ministry = null;

                    Notification notification = NotificationHelper.GenerateDismissedMinisterNotification(user.Username, ministerToDismiss.Username, ministryCode);
                    database.Notifications.Add(notification);

                    database.SaveChanges();

                    return Content("success");
                }
                else
                {
                    return StatusCode(403);
                }
            }
        }

        [HttpPost]
        public IActionResult InviteMinister(string authSessionCookie, string ministry, string invitedUsername)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country country = database.Countries.Single(c => c.CountryName == user.CountryName);

                if (user.Ministry == MinistryHelper.MinistryCode.PrimeMinister)
                {
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                    if (database.Users.Any(u => u.CountryName == country.CountryName && u.Ministry == ministryCode))
                    {
                        return Content("Error: There is already a minister in that ministry. Dismiss them before inviting a new one.");
                    }

                    if (database.InvitedMinisters.Any(im => im.CountryName == country.CountryName && im.Ministry == ministryCode))
                    {
                        return Content("Error: A user is already being invited to this ministry. Wait until they accept or decline the invitation, or cancel it.");
                    }

                    InvitedMinister invitedMinister = new InvitedMinister
                    {
                        Username = invitedUsername,
                        CountryName = country.CountryName,
                        Ministry = ministryCode
                    };
                    database.InvitedMinisters.Add(invitedMinister);

                    Notification notification = NotificationHelper.GenerateMinisterialInvitationNotification(user.Username, invitedMinister.Username, invitedMinister.Ministry);
                    database.Notifications.Add(notification);

                    database.SaveChanges();

                    return Content("success");
                }
                else
                {
                    return StatusCode(403);
                }
            }
        }

        [HttpPost]
        public IActionResult AcceptMinistryInvite(string authSessionCookie, string ministry, string newCountryName, string ministryToReplacePM = null)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country newCountry = database.Countries.Single(c => c.CountryName == newCountryName);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (database.InvitedMinisters.Any(im => im.Username == user.Username && im.CountryName == newCountry.CountryName && im.Ministry == ministryCode))
                {
                    InvitedMinister invitedMinister = database.InvitedMinisters.Single(im => im.Username == user.Username && im.CountryName == newCountry.CountryName && im.Ministry == ministryCode);
                    database.InvitedMinisters.Remove(invitedMinister);

                    if (user.CountryName != null)
                    {
                        if (user.Ministry == MinistryHelper.MinistryCode.PrimeMinister)
                        {
                            Country oldCountry = database.Countries.Single(c => c.CountryName == user.CountryName);

                            bool stillMinisters; // will there still be ministers after the PM leaves? / are there any other ministers in the old country?
                            stillMinisters = database.Users.Any(u => u.CountryName == oldCountry.CountryName);
                            if (!stillMinisters) database.Countries.Remove(oldCountry);

                            else
                            {
                                MinistryHelper.MinistryCode ministryToReplacePMCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministryToReplacePM);

                                if (!database.Users.Any(u => u.CountryName == oldCountry.CountryName && u.Ministry == ministryToReplacePMCode))
                                {
                                    return Content("Error: The minister you are trying to set as Prime Minister doesn't exist.");
                                }

                                User newPrimeMinister = database.Users.Single(u => u.CountryName == oldCountry.CountryName && u.Ministry == ministryToReplacePMCode);
                                newPrimeMinister.Ministry = MinistryHelper.MinistryCode.PrimeMinister;
                            }
                        }
                    }
                    user.CountryName = newCountry.CountryName;
                    user.Ministry = ministryCode;

                    Notification notification = NotificationHelper.GenerateMinisterialInvitationAcceptedNotification(database.Users.Single(u => u.CountryName == newCountry.CountryName && u.Ministry == MinistryHelper.MinistryCode.PrimeMinister).Username, user.Username, ministryCode);
                    database.Notifications.Add(notification);

                    return Content("success");
                }
                else
                {
                    return StatusCode(403);
                }
            }
        }

        [HttpPost]
        public IActionResult DeclineMinistryInvite(string authSessionCookie, string ministry, string newCountryName)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country newCountry = database.Countries.Single(c => c.CountryName == newCountryName);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (database.InvitedMinisters.Any(im => im.Username == user.Username && im.CountryName == newCountry.CountryName && im.Ministry == ministryCode))
                {
                    InvitedMinister invitedMinister = database.InvitedMinisters.Single(im => im.Username == user.Username && im.CountryName == newCountry.CountryName && im.Ministry == ministryCode);
                    database.InvitedMinisters.Remove(invitedMinister);

                    Notification notification = NotificationHelper.GenerateMinisterialInvitationDeclinedNotification(database.Users.Single(u => u.CountryName == newCountry.CountryName && u.Ministry == MinistryHelper.MinistryCode.PrimeMinister).Username, user.Username, ministryCode);
                    database.Notifications.Add(notification);

                    return Content("success");
                }
                else
                {
                    return StatusCode(403);
                }
            }
        }

        [HttpPost]
        public IActionResult CancelMinistryInvite(string authSessionCookie, string ministry)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Country country = database.Countries.Single(c => c.CountryName == user.CountryName);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (user.Ministry == MinistryHelper.MinistryCode.PrimeMinister)
                {
                    InvitedMinister invitedMinister = database.InvitedMinisters.Single(im => im.CountryName == country.CountryName && im.Ministry == ministryCode);
                    database.InvitedMinisters.Remove(invitedMinister);

                    return Content("success");
                }
                else
                {
                    return StatusCode(403);
                }
            }
        }

        [HttpPost]
        public IActionResult SendEmail(string authSessionCookie)
        {
            string[] toUsernames = Request.Form["to"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            string emailSubject = Request.Form["subject"];
            string emailBody = Request.Form["body"];

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Email email = new Email
                {
                    Subject = emailSubject,
                    Body = emailBody
                };
                EntityEntry<Email> emailTracked = database.Emails.Add(email);
                database.SaveChanges();
                emailTracked.Reload();

                foreach (string username in toUsernames)
                {
                    UserEmail userEmail = new UserEmail
                    {
                        EmailId = emailTracked.Entity.EmailId,
                        SendingUsername = user.Username,
                        ReceivingUsername = username
                    };
                    database.UserEmails.Add(userEmail);
                }

                database.SaveChanges();

                return View("CloseWindow");
            }
        }

        [HttpPost]
        public IActionResult MarkEmailAsReadUnread(string authSessionCookie, int emailId, string readOrUnread)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                UserEmail userEmailToMark = database.UserEmails.Single(ue => ue.EmailId == emailId && ue.ReceivingUsername == user.Username);

                switch (readOrUnread)
                {
                    case "read":
                        userEmailToMark.MarkedAsRead = true;
                        break;

                    case "unread":
                        userEmailToMark.MarkedAsRead = false;
                        break;

                    default:
                        return Content("invalid or missing parameter: readOrUnread");
                }

                return Content("success");
            }
        }

        [HttpPost]
        public IActionResult MarkNotificationAsRead(string authSessionCookie, int notificationId)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                Notification notificationToMark = database.Notifications.Single(n => n.NotificationId == notificationId);

                if (notificationToMark.Username != user.Username)
                {
                    return StatusCode(403);
                }

                notificationToMark.MarkedAsRead = true;
                database.SaveChanges();

                return Content("success");
            }
        }
        #endregion

        #region CreateACountry
        public IActionResult CreateACountry(string authSessionCookie)
        {
            ViewData["userLoggedIn"] = true;

            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            using (DatabaseContext database = new DatabaseContext())
            {
                User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                if (user.CountryName != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Game/Index");
                }
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

                using (DatabaseContext database = new DatabaseContext())
                {
                    User user = database.Users.Single(u => u.FirebaseUid == firebaseUid);

                    if (user.CountryName == null)
                    {
                        Country country = new Country
                        {
                            CountryName = Request.Form["country-name"],
                            CapitalName = Request.Form["capital-name"],
                            FlagId = CountryGenerationHelper.FlagNameToId(Request.Form["flag-name"])
                        };

                        if (database.Countries.Any(c => c.CountryName == country.CountryName))
                        {
                            ViewData["errorMessage"] = "There is another country with that name, and we don't allow duplicate country names. Sorry!";
                            return View("../Error/TextError");
                        }

                        database.Countries.Add(country);
                        user.CountryName = country.CountryName;
                        database.SaveChanges();

                        return Redirect("/");
                    }
                    else
                    {
                        ViewData["errorMessage"] = "You are already a minister in another country.";
                        return View("../Error/TextError");
                    }
                }
            }
            else
            {
                ViewData["errorMessage"] = "You are not logged in.";
                return View("../Error/TextError");
            }
        }
        #endregion
    }
}
