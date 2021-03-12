using System.Collections.Generic;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Region
    {
        public string RegionId { get; set; } // format: YX
        public List<Location> Locations { get; set; }

        public int GetRegionX()
        {
            return "ABCDEFGHIJ".IndexOf(this.RegionId[1]);
        }

        public int GetRegionY()
        {
            return "ABCDEFGHIJ".IndexOf(this.RegionId[0]);
        }
    }
}
