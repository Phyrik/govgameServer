using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class InvitedMinister
    {
        [Key]
        public string Username { get; set; }
        [Key]
        public string CountryName { get; set; }
        [Key]
        public string Ministry { get; set; }
    }
}
