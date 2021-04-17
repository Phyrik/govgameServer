using govgameSharedClasses.Models.MongoDB;

namespace govgameSharedClasses.Helpers
{
    public class NotificationHelper
    {
        public static NotificationSendRequest GenerateMinisterialInvitationNotification(string inviterUserId, string invitedUserId, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            PublicUser inviterUser = MongoDBHelper.UsersDatabase.GetPublicUser(inviterUserId);
            Country inviterCountry = MongoDBHelper.CountriesDatabase.GetCountry(inviterUser.CountryId);

            return new NotificationSendRequest
            {
                UserId = invitedUserId,
                Title = $"Invitation to become a minister in {inviterCountry.CountryName}'s government",
                Content = $"The Prime Minister of {inviterCountry.CountryName}, {inviterUser.Username}, has invited you to be their country's {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}. Click this notification to accept or decline.",
                Link = $"/Game/Invite/Minister?countryId={inviterCountry.CountryId}&ministry={ministryInvitedTo}"
            };
        }

        public static NotificationSendRequest GenerateMinisterialInvitationAcceptedNotification(string inviterUserId, string invitedUserId, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            PublicUser invitedUser = MongoDBHelper.UsersDatabase.GetPublicUser(invitedUserId);

            return new NotificationSendRequest
            {
                UserId = inviterUserId,
                Title = $"{invitedUser.Username} has accepted your ministry invitation",
                Content = $"{invitedUser.Username} is now your new {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}."
            };
        }

        public static NotificationSendRequest GenerateMinisterialInvitationDeclinedNotification(string inviterUserId, string invitedUserId, MinistryHelper.MinistryCode ministryInvitedTo)
        {
            PublicUser invitedUser = MongoDBHelper.UsersDatabase.GetPublicUser(invitedUserId);

            return new NotificationSendRequest
            {
                UserId = inviterUserId,
                Title = $"{invitedUser.Username} has declined your ministry invitation",
                Content = $"{invitedUser.Username} has decided not to be your {MinistryHelper.MinistryCodeToMinisterName(ministryInvitedTo)}."
            };
        }

        public static NotificationSendRequest GenerateDismissedMinisterNotification(string dismisserUserId, string dismissedUserId, MinistryHelper.MinistryCode ministryDismissedFrom)
        {
            PublicUser dismisserUser = MongoDBHelper.UsersDatabase.GetPublicUser(dismisserUserId);
            Country dismisserCountry = MongoDBHelper.CountriesDatabase.GetCountry(dismisserUser.CountryId);

            return new NotificationSendRequest
            {
                UserId = dismissedUserId,
                Title = $"You've been dismissed from your ministry",
                Content = $"The Prime Minister of {dismisserCountry.CountryName}, {dismisserUser.Username}, has dismissed you from your position as {MinistryHelper.MinistryCodeToMinisterName(ministryDismissedFrom)}."
            };
        }
    }
}
