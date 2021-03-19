using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace govgameSandboxAndTesting
{
    class Program
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@ec2-35-178-4-240.eu-west-2.compute.amazonaws.com");

        static readonly IMongoDatabase locationsDatabase = mongoClient.GetDatabase("govgame_locations_table");
        static readonly IMongoCollection<Location> locationsCollection = locationsDatabase.GetCollection<Location>("locations");

        static Random random = new Random();

        static void Main(string[] args)
        {
            Color mountain = Color.FromArgb(64, 64, 64);
            Color coal = Color.FromArgb(0, 0, 0);
            Color iron = Color.FromArgb(69, 136, 145);

            Bitmap map = (Bitmap)Image.FromFile("map.png");

            /* Plan:
             * 
             * - loop through squares
             * - if square is mountain:
             *  - get square's neighbours (all 8)
             *  - if any neighbours are not mountain, skip
             *  - if no neighbours are ore, 80% chance that a ore deposit starts
             *  - if any neighbours (4) are ore:
             *   - if neighbours contain >1 type of ore, skip
             *   - if neighbours contain 1 type of ore, 60% chance of continuing ore deposit
             */

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    if (map.GetPixel(x, y).ToArgb() == mountain.ToArgb())
                    {
                        if (x == 0 || x == map.Width - 1 || y == 0 || y == map.Height - 1) continue;

                        List<Color> neighbours = new List<Color>();
                        for (int xOffset = -1; xOffset <= 1; xOffset++)
                        {
                            for (int yOffset = -1; yOffset <= 1; yOffset++)
                            {
                                if (xOffset == 0 && yOffset == 0) continue;

                                neighbours.Add(map.GetPixel(x + xOffset, y + yOffset));
                            }
                        }

                        // if any neighbours aren't mountain (or ore), skip
                        if (neighbours.Where(neighbour => neighbour.ToArgb() != mountain.ToArgb() && neighbour.ToArgb() != coal.ToArgb() && neighbour.ToArgb() != iron.ToArgb()).Count() > 0) continue;

                        // if all neighbours are mountain and not ore
                        if (neighbours.Where(neighbour => neighbour.ToArgb() == mountain.ToArgb()).Count() == 8)
                        {
                            if (random.NextDouble() > 0.2)
                            {
                                if (random.NextDouble() > 0.5)
                                {
                                    map.SetPixel(x, y, coal);
                                }
                                else
                                {
                                    map.SetPixel(x, y, iron);
                                }
                            }
                        }
                        // if any neighbours are ore
                        else
                        {
                            List<Color> neighboursSquare = new List<Color> { neighbours[1], neighbours[3], neighbours[4], neighbours[6] };

                            // if neighbours contain at least one of each ore
                            if (neighboursSquare.Where(neighbour => neighbour.ToArgb() == coal.ToArgb()).Count() > 0 && neighboursSquare.Where(neighbour => neighbour.ToArgb() == iron.ToArgb()).Count() > 0) { continue; }

                            Color oreToSet = mountain;
                            string oreToPrint = "error";

                            if (neighboursSquare.Where(neighbour => neighbour.ToArgb() == coal.ToArgb()).Count() > 0) { oreToSet = coal; oreToPrint = "Coal"; }
                            if (neighboursSquare.Where(neighbour => neighbour.ToArgb() == iron.ToArgb()).Count() > 0) { oreToSet = iron; oreToPrint = "Iron"; }

                            if (random.NextDouble() > 0.1)
                            {
                                map.SetPixel(x, y, oreToSet);
                            }
                        }
                    }
                }

                Console.WriteLine($"{x}/{map.Width} rows completed");
            }

            map.Save("oreMap.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
