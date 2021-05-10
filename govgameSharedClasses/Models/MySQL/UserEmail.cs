namespace govgameSharedClasses.Models.MySQL
{
    public class UserEmail
    {
        public int EmailId { get; set; }
        public string SendingUsername { get; set; }
        public string ReceivingUsername { get; set; }
    }
}
