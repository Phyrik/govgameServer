#nullable enable

namespace govgameSharedClasses.Models.MySQL
{
    public class UserEmail
    {
        public int EmailId { get; set; }
        public string SendingUsername { get; set; } = null!;
        public string ReceivingUsername { get; set; } = null!;
        public bool MarkedAsRead { get; set; }
    }
}
