#nullable enable

using govgameSharedClasses.Helpers;

namespace govgameSharedClasses.Models.MySQL
{
    public class InvitedMinister
    {
        public string Username { get; set; } = null!;
        public string CountryName { get; set; } = null!;
        public MinistryHelper.MinistryCode Ministry { get; set; }
    }
}
