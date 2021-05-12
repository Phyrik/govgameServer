#nullable enable

using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class Country
    {
        [Key]
        public string CountryName { get; set; } = null!;
        public string CapitalName { get; set; } = null!;
        public int FlagId { get; set; }
    }
}
