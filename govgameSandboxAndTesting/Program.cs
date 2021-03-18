using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Drawing;

namespace govgameSandboxAndTesting
{
    class Program
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@ec2-35-178-4-240.eu-west-2.compute.amazonaws.com");

        static readonly IMongoDatabase locationsDatabase = mongoClient.GetDatabase("govgame_locations_table");
        static readonly IMongoCollection<Location> locationsCollection = locationsDatabase.GetCollection<Location>("locations");

        static void Main(string[] args)
        {
            // biomes: "deep water" = rgb(0, 19, 127), "shallow water" = rgb(0, 38, 255), "coast" = rgb(0, 255, 25), "grass" = rgb(0, 163, 16), "mountain" = rgb(64, 64, 64)

            Color deepWater = Color.FromArgb(0, 19, 127);
            Color shallowWater = Color.FromArgb(0, 38, 255);
            Color coast = Color.FromArgb(0, 255, 25);
            Color grass = Color.FromArgb(0, 163, 16);
            Color mountain = Color.FromArgb(64, 64, 64);

            Bitmap map = (Bitmap)Image.FromFile("map.png");

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    string biome = "unknown";

                    Color pixelColour = map.GetPixel(x, y);

                    if (pixelColour.ToArgb() == deepWater.ToArgb())
                    {
                        biome = "deep water";
                    }
                    if (pixelColour.ToArgb() == shallowWater.ToArgb())
                    {
                        biome = "shallow water";
                    }
                    if (pixelColour.ToArgb() == coast.ToArgb())
                    {
                        biome = "coast";
                    }
                    if (pixelColour.ToArgb() == grass.ToArgb())
                    {
                        biome = "grass";
                    }
                    if (pixelColour.ToArgb() == mountain.ToArgb())
                    {
                        biome = "mountain";
                    }

                    Location location = new Location
                    {
                        LocationId = new ObjectId(),
                        GlobalX = x,
                        GlobalY = y,
                        Owner = "none",
                        Biome = biome
                    };

                    locationsCollection.InsertOne(location);
                }

                Console.WriteLine($"{x}/{map.Width} columns completed. ({(double)x/map.Width}%)");
            }
        }
    }
}
