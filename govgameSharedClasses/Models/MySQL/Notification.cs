#nullable enable

using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Username { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? Link { get; set; }
        public bool MarkedAsRead { get; set; }
    }
}
