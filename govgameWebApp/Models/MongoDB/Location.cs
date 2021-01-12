using MongoDB.Bson.Serialization.Attributes;

namespace govgameWebApp.Models.MongoDB
{
    public class Location
    {
        [BsonId]
        public string LocationId { get; set; }
        public string RegionId { get; set; }
        public int RegionalX { get; set; }
        public int RegionalY { get; set; }
        public int GlobalX { get; set; }
        public int GlobalY { get; set; }
        public string Biome { get; set; }
        public string Owner { get; set; }

        public override string ToString()
        {
            return $"{RegionId}:{RegionalX}.{RegionalY}";
        }
    }

    public class RegionalLocationIdentifier
    {
        public string RegionId { get; set; }
        public int RegionalX { get; set; }
        public int RegionalY { get; set; }

        public RegionalLocationIdentifier(string regionId, int x, int y)
        {
            this.RegionId = regionId;
            this.RegionalX = x;
            this.RegionalY = y;
        }
    }

    public class GlobalLocationIdentifier
    {
        public int GlobalX { get; set; }
        public int GlobalY { get; set; }

        public GlobalLocationIdentifier(int x, int y)
        {
            this.GlobalX = x;
            this.GlobalY = y;
        }
    }
    public class LocationsDimensions
    {
        public GlobalLocationIdentifier TopLeft { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class LocationUpdate
    {
        public string Owner { get; set; }
    }
}
