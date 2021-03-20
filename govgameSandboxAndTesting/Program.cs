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

        static void Main(string[] args)
        {
            Bitmap resourceMap = (Bitmap)Image.FromFile("resource map.png");


        }
    }
}
