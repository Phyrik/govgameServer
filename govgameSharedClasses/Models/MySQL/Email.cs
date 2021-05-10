﻿using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class Email
    {
        [Key]
        public int EmailId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool MarkedAsRead { get; set; }
    }
}
