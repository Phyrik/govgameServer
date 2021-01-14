using System.Collections.Generic;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Region
    {
        public string RegionId { get; set; } // format: YX
        public List<Location> Locations { get; set; }

        public Location this[int x, int y]
        {
            get { return this.Locations.Find(location => location.RegionalX == x && location.RegionalY == y); }
            set
            {
                int locationIndex = y * (2000 / 10) + x;
                this.Locations[locationIndex] = value;
            }
        }

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
