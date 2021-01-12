using System;
using System.Collections.Generic;
using System.Linq;

namespace govgameWebApp.Models.MongoDB
{
    public class Map
    {
        public List<Region> Regions { get; set; }

        public Region this[string regionId]
        {
            get
            {
                foreach (Region region in Regions)
                {
                    if (region.RegionId == regionId)
                    {
                        return region;
                    }
                }

                return null;
            }
        }
    }
}
