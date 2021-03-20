using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace govgameSharedClasses.Models.MongoDB
{
    public class Location
    {
        [BsonId]
        public ObjectId LocationId { get; set; }
        public int GlobalX { get; set; }
        public int GlobalY { get; set; }
        public string Owner { get; set; }
        // "deep water", "shallow water", "coast", "grass", "mountain"
        public string Biome { get; set; }
        public int Coal { get; set; }
        public int Hydrocarbons { get; set; }
        public int Iron { get; set; }
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
