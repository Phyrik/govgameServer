﻿using FirebaseAdmin.Auth;
using govgameSharedClasses.Models.MongoDB;

namespace govgameSharedClasses.Helpers
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

            if (MongoDBHelper.UsersDatabase.NewUser(publicUser))
            {
                return MongoDBHelper.UsersDatabase.GetPublicUser(firebaseToken.Uid);
            }
            else
            {
                return null;
            }
        }
    }
}
