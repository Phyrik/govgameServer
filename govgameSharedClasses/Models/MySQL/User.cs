#nullable enable

using govgameSharedClasses.Helpers;
using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class User
    {
        [Key]
        public string Username { get; set; } = null!;
        public string FirebaseUid { get; set; } = null!;
        public string? CountryName { get; set; }
        public MinistryHelper.MinistryCode? Ministry { get; set; }
        public bool Admin { get; set; }
    }
}
