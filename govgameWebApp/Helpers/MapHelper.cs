using govgameSharedClasses.Models.MongoDB;
using System;
using System.Drawing;
using System.Linq;

namespace govgameWebApp.Helpers
{
    public class MapHelper
    {
        public static readonly int worldMapWidth = 2000;
        public static readonly int worldMapHeight = 2000;
        public static readonly int worldRegionsHori = 10;
        public static readonly int worldRegionsVert = 10;
        public static readonly int worldRegionWidth = worldMapWidth / worldRegionsHori;
        public static readonly int worldRegionHeight = worldMapHeight / worldRegionsVert;
        public static readonly string worldRegionChars = "ABCDEFGHIJ";
    }

    public class MapColourHelper
    {
        public static class Colours
        {
            public static readonly int OverlayAlpha = 200;
            public static readonly Color OwnCountry = Color.FromArgb(96, 130, 182);

            public static class Biomes
            {
                public static readonly Color Water = Color.FromArgb(16, 27, 125);
                public static readonly Color Grass = Color.FromArgb(51, 161, 59);
                public static readonly Color Forest = Color.FromArgb(29, 74, 23);
                public static readonly Color Mountain = Color.FromArgb(120, 120, 120);
            }
        }

        public static Color ApplyOverlayAlpha(Color color)
        {
            return Color.FromArgb(Colours.OverlayAlpha, color);
        }
    }

    public class LocationHelper
    {
        private class LocationIdentifier
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public static GlobalLocationIdentifier[] GetGLIsFromLocations(Location[] locations)
        {
            GlobalLocationIdentifier[] globalLocationIdentifiers = Array.ConvertAll(locations, location => new GlobalLocationIdentifier(location.GlobalX, location.GlobalY));

            return globalLocationIdentifiers;
        }

        public static LocationsDimensions GetDimensionsOfLocations(GlobalLocationIdentifier[] globalLocationIdentifiers)
        {
            LocationIdentifier topLeftLocationIdentifier = new LocationIdentifier
            {
                X = globalLocationIdentifiers.Min(globalLocationIdentifier => globalLocationIdentifier.GlobalX),
                Y = globalLocationIdentifiers.Min(globalLocationIdentifier => globalLocationIdentifier.GlobalY)
            };
            LocationIdentifier bottomRightLocationIdentifier = new LocationIdentifier
            {
                X = globalLocationIdentifiers.Max(globalLocationIdentifier => globalLocationIdentifier.GlobalX),
                Y = globalLocationIdentifiers.Max(globalLocationIdentifier => globalLocationIdentifier.GlobalY)
            };

            return new LocationsDimensions
            {
                TopLeft = new GlobalLocationIdentifier(topLeftLocationIdentifier.X, topLeftLocationIdentifier.Y),
                Width = bottomRightLocationIdentifier.X - topLeftLocationIdentifier.X,
                Height = bottomRightLocationIdentifier.Y - topLeftLocationIdentifier.Y
            };
        }
    }
}
