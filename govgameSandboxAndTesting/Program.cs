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

        static Color deepWater = Color.FromArgb(0, 19, 127);
        const int deepWaterArgb = -16772225;
        static Color shallowWater = Color.FromArgb(0, 38, 255);
        const int shallowWaterArgb = -16767233;
        static Color coast = Color.FromArgb(0, 255, 25);
        const int coastArgb = -16711911;
        static Color grass = Color.FromArgb(0, 163, 16);
        const int grassArgb = -16735472;
        static Color mountain = Color.FromArgb(64, 64, 64);
        const int mountainArgb = -12566464;

        static Color hydrocarbons = Color.FromArgb(255, 216, 0);
        const int hydrocarbonsArgb = -10240;
        static Color iron = Color.FromArgb(69, 102, 124);
        const int ironArgb = -12228996;
        static Color clay = Color.FromArgb(127, 51, 0);
        const int clayArgb = -8441088;

        static Random random = new Random();

        static void Main(string[] args)
        {
            Bitmap map = (Bitmap)Image.FromFile("map.png");

            /*
             * 1st round: choose centre locations for deposits
             * 2nd round: expand those deposits
             * 
             * 1st round details:
             *  - chances for starting a deposit:
             *   - deep water:
             *    - hydrocarbons: 25%
             *    - iron: 0%
             *    - clay: 0%
             *    - nothing: 75%
             *   - shallow water:
             *    - hydrocarbons: 50%
             *    - iron: 0%
             *    - clay: 5%
             *    - nothing: 45%
             *   - coast:
             *    - hydrocarbons: 0%
             *    - iron: 0%
             *    - clay: 50%
             *    - nothing: 50%
             *   - grass:
             *    - hydrocarbons: 5%
             *    - iron: 10%
             *    - clay: 40%
             *    - nothing: 45%
             *   - mountain:
             *    - hydrocarbons: 0%
             *    - iron: 90%
             *    - clay: 0%
             *    - nothing: 10%
             * 
             * 2nd round details:
             *  - 
             * 
             */

            int hydrocarbonDeposits = 0;
            int ironDeposits = 0;
            int clayDeposits = 0;
            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    double randomDouble = random.NextDouble();

                    Color colourToSet = map.GetPixel(x, y);
                    switch (map.GetPixel(x, y).ToArgb())
                    {
                        case deepWaterArgb:
                            if (randomDouble < (0.25 / 1000))
                            {
                                colourToSet = hydrocarbons;
                            }
                            break;

                        case shallowWaterArgb:
                            if (randomDouble < (0.5 / 1000))
                            {
                                colourToSet = hydrocarbons;
                            }
                            else if (randomDouble < (0.55 / 1000))
                            {
                                colourToSet = clay;
                            }
                            break;

                        case coastArgb:
                            if (randomDouble < (0.5 / 1000))
                            {
                                colourToSet = clay;
                            }
                            break;

                        case grassArgb:
                            if (randomDouble < (0.05 / 1000))
                            {
                                colourToSet = hydrocarbons;
                            }
                            else if (randomDouble < (0.15 / 1000))
                            {
                                colourToSet = iron;
                            }
                            else if (randomDouble < (0.55 / 1000))
                            {
                                colourToSet = clay;
                            }
                            break;

                        case mountainArgb:
                            if (randomDouble < (0.9 / 1000))
                            {
                                colourToSet = iron;
                            }
                            break;

                        default:
                            break;
                    }

                    switch (colourToSet.ToArgb())
                    {
                        case hydrocarbonsArgb:
                            hydrocarbonDeposits++;
                            break;

                        case ironArgb:
                            ironDeposits++;
                            break;

                        case clayArgb:
                            clayDeposits++;
                            break;

                        default:
                            break;
                    }

                    map.SetPixel(x, y, colourToSet);
                }

                Console.WriteLine($"{x}/{map.Width} rows completed.");
            }

            map.Save("resourceDepositCentres.png", System.Drawing.Imaging.ImageFormat.Png);

            Bitmap tempResourceMap = new Bitmap(map);
            for (int i = 0; i < 5; i++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        List<int[]> fourNeighbours = new List<int[]>();
                        List<int[]> eightNeighbours = new List<int[]>();

                        if (x == 0 || x == map.Width - 1 || y == 0 || y == map.Height - 1) continue;

                        for (int xOffset = -1; xOffset < 2; xOffset++)
                        {
                            for (int yOffset = -1; yOffset < 2; yOffset++)
                            {
                                if (xOffset == 0 && yOffset == 0) continue;

                                eightNeighbours.Add(new int[] { x + xOffset, y + yOffset });

                                if (Math.Abs(xOffset) == Math.Abs(yOffset)) continue;

                                fourNeighbours.Add(new int[] { x + xOffset, y + yOffset });
                            }
                        }

                        switch (map.GetPixel(x, y).ToArgb())
                        {
                            case hydrocarbonsArgb:
                                if (eightNeighbours.Where(neighbour => map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == ironArgb || map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == clayArgb).Count() == 0)
                                {
                                    foreach (int[] neighbour in fourNeighbours)
                                    {
                                        tempResourceMap.SetPixel(neighbour[0], neighbour[1], hydrocarbons);
                                    }
                                }
                                break;

                            case ironArgb:
                                if (eightNeighbours.Where(neighbour => map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == hydrocarbonsArgb || map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == clayArgb).Count() == 0)
                                {
                                    foreach (int[] neighbour in fourNeighbours)
                                    {
                                        tempResourceMap.SetPixel(neighbour[0], neighbour[1], iron);
                                    }
                                }
                                break;

                            case clayArgb:
                                if (eightNeighbours.Where(neighbour => map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == hydrocarbonsArgb || map.GetPixel(neighbour[0], neighbour[1]).ToArgb() == ironArgb).Count() == 0)
                                {
                                    foreach (int[] neighbour in fourNeighbours)
                                    {
                                        tempResourceMap.SetPixel(neighbour[0], neighbour[1], clay);
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    if ((x + 1) % 100 == 0) Console.WriteLine($"{x + 1}/{map.Width} rows completed.");
                }

                map = new Bitmap(tempResourceMap);

                Console.WriteLine($"{i + 1}/{5} rounds completed.");

                map.Save($"resourceDepositsExpanded{i + 1}.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            map.Save("resourceDepositsExpandedOnce.png", System.Drawing.Imaging.ImageFormat.Png);

            Console.WriteLine($"{hydrocarbonDeposits} hydrocarbon deposits; {ironDeposits} iron deposits; {clayDeposits} clay deposits");
        }
    }
}
