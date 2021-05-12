using govgameSharedClasses.Helpers.MySQL;
using govgameSharedClasses.Models.MySQL;
using System.Linq;

namespace govgameSharedClasses.Helpers
{
    public class NotificationHelper
    {
        public static Notification GenerateMinisterialInvitationNotification(string inviterUsername, string invitedUsername, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            using (DatabaseContext database = new DatabaseContext())
            {
                User inviterUser = database.Users.Single(u => u.Username == inviterUsername);
                Country inviterCountry = database.Countries.Single(c => c.CountryName == inviterUser.CountryName);

                return new Notification
                {
                    Username = invitedUsername,
                    Title = $"Invitation to become a minister in {inviterCountry.CountryName}'s government",
                    Content = $"The Prime Minister of {inviterCountry.CountryName}, {inviterUser.Username}, has invited you to be their country's {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}. Click this notification to accept or decline.",
                    Link = $"/Game/Invite/Minister?countryName={inviterCountry.CountryName}&ministry={ministryInvitedTo}"
                };
            }
        }

        public static Notification GenerateMinisterialInvitationAcceptedNotification(string inviterUsername, string invitedUsername, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            using (DatabaseContext database = new DatabaseContext())
            {
                User invitedUser = database.Users.Single(u => u.Username == invitedUsername);

                return new Notification
                {
                    Username = inviterUsername,
                    Title = $"{invitedUser.Username} has accepted your ministry invitation",
                    Content = $"{invitedUser.Username} is now your new {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}."
                };
            }
        }

        public static Notification GenerateMinisterialInvitationDeclinedNotification(string inviterUsername, string invitedUsername, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            using (DatabaseContext database = new DatabaseContext())
            {
                User invitedUser = database.Users.Single(u => u.Username == invitedUsername);

                return new Notification
                {
                    Username = inviterUsername,
                    Title = $"{invitedUser.Username} has declined your ministry invitation",
                    Content = $"{invitedUser.Username} has decided not to be your {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}."
                };
            }
        }

        public static Notification GenerateDismissedMinisterNotification(string dismisserUsername, string dismissedUsername, MinistryHelper.MinistryCode ministryDismissedFrom)
        {
            using (DatabaseContext database = new DatabaseContext())
            {
                User dismisserUser = database.Users.Single(u => u.Username == dismisserUsername);
                Country dismisserCountry = database.Countries.Single(c => c.CountryName == dismisserUser.CountryName);

                return new Notification
                {
                    Username = dismissedUsername,
                    Title = $"You've been dismissed from your ministry",
                    Content = $"The Prime Minister of {dismisserCountry.CountryName}, {dismisserUser.Username}, has dismissed you from your position as {MinistryHelper.MinistryCodeToMinisterName(ministryDismissedFrom)}."
                };
            }
        }
    }
}
