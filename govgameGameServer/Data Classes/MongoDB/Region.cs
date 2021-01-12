using System.Collections.Generic;

namespace govgameWebApp.Models.MongoDB
{
    public class Region
    {
        public string RegionId { get; set; } // format: YX
        public List<Location> Locations { get; set; }
    }
}
