using static govgameSharedClasses.Helpers.MinistryHelper;

namespace govgameSharedClasses.Models.MySQL
{
    public class User
    {
        public string Username { get; set; }
        public string FirebaseUid { get; set; }
        public string CountryName { get; set; }
        public MinistryCode Ministry { get; set; }
        public bool Admin { get; set; }
    }
}
