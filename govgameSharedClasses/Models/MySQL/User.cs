using govgameSharedClasses.Helpers;
using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string FirebaseUid { get; set; }
        public string CountryName { get; set; }
        public MinistryHelper.MinistryCode Ministry { get; set; }
        public bool Admin { get; set; }
    }
}
