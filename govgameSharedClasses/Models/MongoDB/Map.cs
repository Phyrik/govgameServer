using govgameWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace govgameSharedClasses.Models.MongoDB
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

        public string GetTopLeftRegionId()
        {
            int leftMostRegionX = Regions.Min(region => region.GetRegionX());
            int topMostRegionY = Regions.Min(region => region.GetRegionY());

            return $"{MapHelper.worldRegionChars[topMostRegionY]}{MapHelper.worldRegionChars[leftMostRegionX]}";
        }

        public int GetRegionsHori()
        {
            int leftMostRegionX = Regions.Min(region => region.GetRegionX());
            int rightMostRegionX = Regions.Max(region => region.GetRegionX());

            return rightMostRegionX - leftMostRegionX + 1;
        }

        public int GetRegionsVert()
        {
            int topMostRegionY = Regions.Min(region => region.GetRegionY());
            int bottomMostRegionY = Regions.Max(region => region.GetRegionY());

            return bottomMostRegionY - topMostRegionY + 1;
        }
    }
}
