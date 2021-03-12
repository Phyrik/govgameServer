using govgameSharedClasses.Models.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Drawing;

namespace govgameSandboxAndTesting
{
    public class EarthMapUploader
    {
        static readonly string username = "ASPNETwebapp";
        static readonly string password = "deUM3YhG9HreNCQN";
        static readonly MongoClient mongoClient = new MongoClient($"mongodb://{username}:{password}@ec2-35-178-4-240.eu-west-2.compute.amazonaws.com");
        static readonly IMongoDatabase mapDatabase = mongoClient.GetDatabase("govgame_map_table");
        static readonly IMongoCollection<Location> locationsCollection = mapDatabase.GetCollection<Location>("locations_testing");

        public class Colours
        {
            public static Color Af = Color.FromArgb(0, 0, 255);
            public static Color Am = Color.FromArgb(0, 120, 255);
            public static Color AwAs = Color.FromArgb(70, 170, 250);
            public static Color BWh = Color.FromArgb(255, 0, 0);
            public static Color BWk = Color.FromArgb(255, 150, 150);
            public static Color BSh = Color.FromArgb(245, 165, 0);
            public static Color BSk = Color.FromArgb(255, 220, 100);
            public static Color Csa = Color.FromArgb(255, 255, 0);
            public static Color Csb = Color.FromArgb(200, 200, 0);
            public static Color Csc = Color.FromArgb(150, 150, 0);
            public static Color Cwa = Color.FromArgb(150, 255, 150);
            public static Color Cwb = Color.FromArgb(100, 200, 100);
            public static Color Cwc = Color.FromArgb(50, 150, 50);
            public static Color Cfa = Color.FromArgb(200, 255, 80);
            public static Color Cfb = Color.FromArgb(100, 255, 80);
            public static Color Cfc = Color.FromArgb(50, 200, 0);
            public static Color Dsa = Color.FromArgb(255, 0, 255);
            public static Color Dsb = Color.FromArgb(200, 0, 200);
            public static Color Dsc = Color.FromArgb(150, 50, 150);
            public static Color Dsd = Color.FromArgb(150, 100, 150);
            public static Color Dwa = Color.FromArgb(170, 175, 255);
            public static Color Dwb = Color.FromArgb(89, 120, 220);
            public static Color Dwc = Color.FromArgb(75, 80, 179);
            public static Color Dwd = Color.FromArgb(50, 0, 135);
            public static Color Dfa = Color.FromArgb(0, 255, 255);
            public static Color Dfb = Color.FromArgb(55, 200, 255);
            public static Color Dfc = Color.FromArgb(0, 125, 125);
            public static Color Dfd = Color.FromArgb(0, 70, 95);
            public static Color ET = Color.FromArgb(178, 178, 178);
            public static Color EF = Color.FromArgb(102, 102, 102);
            public static Color Water = Color.FromArgb(0, 0, 0, 0);

            public static int numOfColours = 31;

            public static Color[] allClimateColours = new Color[] { Af, Am, AwAs, BWh, BWk, BSh, BSk, Csa, Csb, Csc, Cwa, Cwb, Cwc, Cfa, Cfb, Cfc, Dsa, Dsb, Dsc, Dsd, Dwa, Dwb, Dwc, Dwd, Dfa, Dfb, Dfc, Dfd, ET, EF, Water };
            public static string[] allClimateColourNames = new string[] { "Af", "Am", "AwAs", "BWh", "BWk", "BSh", "BSk", "Csa", "Csb", "Csc", "Cwa", "Cwb", "Cwc", "Cfa", "Cfb", "Cfc", "Dsa", "Dsb", "Dsc", "Dsd", "Dwa", "Dwb", "Dwc", "Dwd", "Dfa", "Dfb", "Dfc", "Dfd", "ET", "EF", "Water" };
        }

        public static void Start()
        {
            Bitmap worldClimateMap = (Bitmap)Image.FromFile("Köppen-Geiger_Climate_Classification_Map_(1980–2016)_no_borders.png");

            for (int x = 0; x < worldClimateMap.Width; x++)
            {
                for (int y = 0; y < worldClimateMap.Height; y++)
                {
                    for (int i = 0; i < Colours.numOfColours; i++)
                    {
                        if (worldClimateMap.GetPixel(x, y).ToArgb() == Colours.allClimateColours[i].ToArgb())
                        {
                            Location newLocation = new Location { LocationId = new ObjectId(), GlobalX = x, GlobalY = y, Climate = Colours.allClimateColourNames[i], Owner = "none" };

                            locationsCollection.InsertOne(newLocation);
                        }
                    }
                }

                Console.WriteLine($"{x + 1}/{worldClimateMap.Width} rows written to database. ({Math.Round((double)(x + 1) / worldClimateMap.Width * 100, 2)}%)");
            }
        }
    }
}
