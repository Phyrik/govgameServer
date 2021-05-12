#nullable enable

using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class MinistryEnum
    {
        [Key]
        public string MinistryName { get; set; } = null!;
    }
}
