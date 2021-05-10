using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class UserEmail
    {
        [Key]
        public int EmailId { get; set; }
        [Key]
        public string SendingUsername { get; set; }
        [Key]
        public string ReceivingUsername { get; set; }
    }
}
