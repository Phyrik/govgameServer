namespace govgameSharedClasses.Models.MySQL
{
    public class Email
    {
        public int EmailId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool MarkedAsRead { get; set; }
    }
}
