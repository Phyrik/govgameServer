using System.ComponentModel.DataAnnotations;

namespace govgameSharedClasses.Models.MySQL
{
    public class Country
    {
        [Key]
        public string CountryName { get; set; }
        public string CapitalName { get; set; }
        public int FlagId { get; set; }
    }
}
