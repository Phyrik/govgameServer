using FirebaseAdmin.Auth;
using govgameSharedClasses.Helpers;
using govgameSharedClasses.Models.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
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

                controller.ViewData["userLoggedIn"] = userLoggedIn;

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
            return Redirect("/Game/Home");
        }

        public IActionResult Home(string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            switch (publicUser.IsAMinister())
            {
                case true:
                    return View();

                case false:
                    return View("./NoCountry/Home");
            }
        }

        public IActionResult Notifications(string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
            {
                ViewData["noCountry"] = true;
            }
            else
            {
                ViewData["noCountry"] = false;
            }

            Notification[] notifications = MongoDBHelper.NotificationsDatabase.GetUsersReceivedNotifications(firebaseUid);
            ViewData["notifications"] = notifications;

            return View();
        }

        public IActionResult Emails(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
            {
                ViewData["noCountry"] = true;
            }
            else
            {
                ViewData["noCountry"] = false;
            }

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
                    return View("404");
            }
        }

        public IActionResult Invite(string page, string authSessionCookie, string countryId, string ministry)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
            {
                ViewData["noCountry"] = true;
            }
            else
            {
                ViewData["noCountry"] = false;
            }

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

                    bool isPrimeMinister = publicUser.IsAPrimeMinister();
                    ViewData["isPrimeMinister"] = isPrimeMinister;

                    bool noMinistersToReplace = true;
                    foreach (MinistryHelper.MinistryCode ministryCodeLoop in Enum.GetValues(typeof(MinistryHelper.MinistryCode)))
                    {
                        if (ministryCodeLoop == MinistryHelper.MinistryCode.PrimeMinister || ministryCodeLoop == MinistryHelper.MinistryCode.None) continue;
                        if (oldCountry.GetMinisterIdByCode(ministryCodeLoop) != "none") noMinistersToReplace = false;
                    }
                    ViewData["noMinistersToReplace"] = noMinistersToReplace;

                    if (newCountry.GetInvitedMinisterIdByCode(ministryCode) != publicUser.UserId)
                    {
                        ViewData["errorMessage"] = "Invalid invite link. The person who invited you may have cancelled the invitation.";
                        return View("../Error/TextError");
                    }

                    return View("./Invite/Minister");

                default:
                    return View("404");
            }
        }

        public IActionResult CountryDashboard(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
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

        public IActionResult PrimeMinisterDashboard(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
            {
                return Redirect("/Game/Index");
            }

            ViewData["publicUser"] = publicUser;

            Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

            ViewData["country"] = country;

            if (!publicUser.HasAccessToMinistry(MinistryHelper.MinistryCode.PrimeMinister))
            {
                return Content("403");
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
                    return View("404");
            }
        }

        public IActionResult FinanceAndTradeDashboard(string page, string authSessionCookie)
        {
            FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
            string firebaseUid = firebaseToken.Uid;

            PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

            if (!publicUser.IsAMinister())
            {
                return Redirect("/Game/Index");
            }

            ViewData["publicUser"] = publicUser;

            Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

            ViewData["country"] = country;

            if (!publicUser.HasAccessToMinistry(MinistryHelper.MinistryCode.FinanceAndTrade))
            {
                return Content("403");
            }

            switch (page)
            {
                case null:
                    return View("./FinanceAndTradeDashboard/Index");

                case "Budget":
                    return View("./FinanceAndTradeDashboard/Budget");

                case "ChangeMinisterialBudget":
                    MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), Request.Query["minister"]);
                    ViewData["ministryCode"] = ministryCode;

                    return View("./FinanceAndTradeDashboard/ChangeMinisterialBudget");

                default:
                    return View("404");
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

                    if (country.GetMinisterIdByCode(ministryCode) == "none")
                    {
                        return Content("Error: There is no minister to dismiss!");
                    }

                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetMinisterIdByCode(ministryCode, "none");

                    UserUpdate userUpdate = new UserUpdate { CountryId = "none" };

                    NotificationSendRequest notificationSendRequest = new NotificationSendRequest
                    {
                        UserId = country.GetMinisterIdByCode(ministryCode),
                        Title = $"Dismissal from your ministerial role at {country.CountryName}",
                        Content = $"{publicUser.Username} has dismissed you from your position as {MinistryHelper.MinistryCodeToMinisterName(ministryCode)} of {country.CountryName}.",
                    };

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(country.CountryId, countryUpdate) &&
                        MongoDBHelper.UsersDatabase.UpdateUser(country.GetMinisterIdByCode(ministryCode), userUpdate) &&
                        govgameGameServer.Managers.MongoDBManager.SendNotification(notificationSendRequest))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("Error: Internal server error.");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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

                    if (country.GetMinisterIdByCode(ministryCode) != "none")
                    {
                        return Content("Error: There is already a minister in that ministry. Dismiss them before inviting a new one.");
                    }

                    if (country.GetInvitedMinisterIdByCode(ministryCode) != "none")
                    {
                        return Content("Error: This user is already being invited to this ministry. Wait until they accept or decline the invitation.");
                    }

                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetInvitedMinisterIdByCode(ministryCode, invitedUserId);

                    NotificationSendRequest notificationSendRequest = new NotificationSendRequest
                    {
                        UserId = invitedUserId,
                        Title = $"Invitation to ministerial role at {country.CountryName}",
                        Content = $"{publicUser.Username} has invited you to become the {MinistryHelper.MinistryCodeToMinisterName(ministryCode)} of {country.CountryName}",
                        Link = $"https://govgame.crumble-technologies.co.uk/Game/Invite/Minister?countryId={country.CountryId}&ministry={ministry}"
                    };

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(country.CountryId, countryUpdate) &&
                        govgameGameServer.Managers.MongoDBManager.SendNotification(notificationSendRequest))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("Error: Internal server error.");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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

                    UserUpdate userUpdate = new UserUpdate { CountryId = newCountry.CountryId };

                    if (publicUser.IsAMinister())
                    {
                        Country oldCountry = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                        CountryUpdate oldCountryUpdate = new CountryUpdate();
                        oldCountryUpdate.SetMinisterIdByCode(publicUser.GetMinistry(), "none");

                        if (publicUser.IsAPrimeMinister())
                        {
                            bool stillMinisters = false;
                            foreach (MinistryHelper.MinistryCode ministryCodeLoop in Enum.GetValues(typeof(MinistryHelper.MinistryCode)))
                            {
                                if (ministryCodeLoop == MinistryHelper.MinistryCode.PrimeMinister || ministryCodeLoop == MinistryHelper.MinistryCode.None) continue;
                                if (oldCountry.GetMinisterIdByCode(ministryCodeLoop) != "none") stillMinisters = true;
                            }
                            if (!stillMinisters) oldCountryUpdate.DeleteCountry = true;

                            if (stillMinisters)
                            {
                                MinistryHelper.MinistryCode ministryToReplacePMCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministryToReplacePM);

                                if (oldCountry.GetMinisterIdByCode(ministryToReplacePMCode) == "none")
                                {
                                    return Content("Error: The minister you are trying to set as Prime Minister doesn't exist.");
                                }

                                oldCountryUpdate.SetMinisterIdByCode(MinistryHelper.MinistryCode.PrimeMinister, oldCountry.GetMinisterIdByCode(ministryToReplacePMCode));
                                oldCountryUpdate.SetMinisterIdByCode(ministryToReplacePMCode, "none");
                            }
                        }

                        if (!MongoDBHelper.CountriesDatabase.UpdateCountry(oldCountry.CountryId, oldCountryUpdate))
                        {
                            return Content("Error: Internal server error.");
                        }
                    }

                    PublicUser newPrimeMinister = MongoDBHelper.UsersDatabase.GetPublicUser(newCountry.PrimeMinisterId);
                    NotificationSendRequest notificationSendRequest = new NotificationSendRequest
                    {
                        UserId = newPrimeMinister.UserId,
                        Title = $"{publicUser.Username} has accepted your ministerial invite",
                        Content = $"{publicUser.Username} has accepted your invitation to become the {MinistryHelper.MinistryCodeToMinisterName(ministryCode)} of your country."
                    };

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(newCountry.CountryId, newCountryUpdate) &&
                        MongoDBHelper.UsersDatabase.UpdateUser(publicUser.UserId, userUpdate) &&
                        govgameGameServer.Managers.MongoDBManager.SendNotification(notificationSendRequest))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("Error: Internal server error.");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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
                    
                    PublicUser newPrimeMinister = MongoDBHelper.UsersDatabase.GetPublicUser(newCountry.PrimeMinisterId);
                    NotificationSendRequest notificationSendRequest = new NotificationSendRequest
                    {
                        UserId = newPrimeMinister.UserId,
                        Title = $"{publicUser.Username} has declined your ministerial invite",
                        Content = $"{publicUser.Username} has declined your invitation to become the {MinistryHelper.MinistryCodeToMinisterName(ministryCode)} of your country."
                    };

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(newCountryId, countryUpdate) &&
                        govgameGameServer.Managers.MongoDBManager.SendNotification(notificationSendRequest))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("Error: Internal server error.");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
            }
        }

        [HttpPost]
        public IActionResult CancelMinistryInvite(string ministry)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (country.PrimeMinisterId == publicUser.UserId)
                {
                    CountryUpdate countryUpdate = new CountryUpdate();
                    countryUpdate.SetInvitedMinisterIdByCode(ministryCode, "none");

                    if (MongoDBHelper.CountriesDatabase.UpdateCountry(country.CountryId, countryUpdate))
                    {
                        return Content("success");
                    }
                    else
                    {
                        return Content("Error: Internal server error.");
                    }
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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
                    ViewData["errorMessage"] = "Internal server error.";
                    return View("../Error/TextError");
                }
            }
            else
            {
                ViewData["errorMessage"] = "You are not logged in.";
                return View("../Error/TextError");
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
                            return Content("Error: Internal server error.");
                        }

                    case "unread":
                        bool markEmailAsUnreadSuccess = MongoDBHelper.EmailsDatabase.MarkEmailAsUnread(emailId);

                        if (markEmailAsUnreadSuccess)
                        {
                            return Content("success");
                        }
                        else
                        {
                            return Content("Error: Internal server error.");
                        }

                    default:
                        return Content("invalid or missing parameter: readOrUnread");
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
            }
        }

        [HttpPost]
        public IActionResult BlockEmailsFromUser(string blockUserId)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                List<string> newBlockedUsersList = new List<string>(publicUser.BlockedUsers);
                newBlockedUsersList.Add(blockUserId);

                UserUpdate userUpdate = new UserUpdate
                {
                    BlockedUsers = newBlockedUsersList.ToArray()
                };

                bool blockUserSuccess = MongoDBHelper.UsersDatabase.UpdateUser(firebaseUid, userUpdate);

                if (blockUserSuccess)
                {
                    return Content("success");
                }
                else
                {
                    return Content("Error: Internal server error.");
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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
                    return Content("Error: Internal server error.");
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
            }
        }

        [HttpPost]
        public IActionResult ChangeMinisterialBudget(string ministry, int changeAmount)
        {
            string authSessionCookie = Request.Cookies["authSession"];

            bool userLoggedIn = FirebaseAuthHelper.IsUserLoggedIn(authSessionCookie);

            if (userLoggedIn)
            {
                FirebaseToken firebaseToken = FirebaseAuth.DefaultInstance.VerifySessionCookieAsync(authSessionCookie).Result;
                string firebaseUid = firebaseToken.Uid;

                PublicUser publicUser = MongoDBHelper.UsersDatabase.GetPublicUser(firebaseUid);

                Country country = MongoDBHelper.CountriesDatabase.GetCountry(publicUser.CountryId);

                MinistryHelper.MinistryCode ministryCode = (MinistryHelper.MinistryCode)Enum.Parse(typeof(MinistryHelper.MinistryCode), ministry);

                if (publicUser.HasAccessToMinistry(MinistryHelper.MinistryCode.FinanceAndTrade))
                {
                    throw new NotImplementedException();
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return Content("Error: You are not logged in.");
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

            if (!publicUser.IsAMinister())
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

                if (!publicUser.IsAMinister())
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
                        ViewData["errorMessage"] = "There is another country with that name, and we don't allow duplicate names. Sorry!";
                        return View("../Error/TextError");
                    }

                    GlobalLocationIdentifier globalLocationIdentifier = new GlobalLocationIdentifier(int.Parse(Request.Form["locationX"]) - 50, int.Parse(Request.Form["locationY"]) - 50);

                    Location[] locations = MongoDBHelper.LocationsDatabase.GetLocations(globalLocationIdentifier, 100, 100);
                    foreach (Location location in locations)
                    {
                        if (location.Owner != "none")
                        {
                            ViewData["errorMessage"] = "Some or all of the land that you chose is owned by another country. Sorry!";
                            return View("../Error/TextError");
                        }
                    }

                    LocationUpdate locationUpdate = new LocationUpdate
                    {
                        Owner = country.CountryId
                    };
                    if (MongoDBHelper.LocationsDatabase.UpdateLocations(globalLocationIdentifier, 100, 100, locationUpdate))
                    {
                        MongoDBHelper.CountriesDatabase.NewCountry(country);

                        MongoDBHelper.UsersDatabase.UpdateUser(firebaseUid, new UserUpdate
                        {
                            CountryId = country.CountryId
                        });

                        return Redirect("/");
                    }
                    else
                    {
                        ViewData["errorMessage"] = "Internal server error.";
                        return View("../Error/TextError");
                    }
                }
                else
                {
                    ViewData["errorMessage"] = "You are already a minister in another country.";
                    return View("../Error/TextError");
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
