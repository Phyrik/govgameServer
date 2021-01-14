using FirebaseAdmin.Auth;
using govgameSharedClasses.Models.MongoDB;

namespace govgameWebApp.Helpers
{
    public class AuthHelper
    {
        public static PublicUser CreateInitialUserInfoDocument(FirebaseToken firebaseToken, string username)
        {
            PublicUser publicUser = new PublicUser
            {
                UserId = firebaseToken.Uid,
                Username = username,
                OwnsCountry = false,
                IsMinister = false,
                CountryId = "none"
            };

            if (MongoDBHelper.NewUser(publicUser))
            {
                return MongoDBHelper.GetPublicUser(firebaseToken.Uid);
            }
            else
            {
                return null;
            }
        }
    }
}
