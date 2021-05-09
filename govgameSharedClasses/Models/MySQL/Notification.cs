namespace govgameSharedClasses.Models.MySQL
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public bool MarkedAsRead { get; set; }
    }
}
