using System;
using System.Linq;
using System.IO;
using GeoCoordinatePortable;
using LoggingKata.API;
using LoggingKata.Models;
using LoggingKata.Logger;
using System.Collections.Generic;
using DotNetEnv;

namespace LoggingKata;

class Program
{
    static readonly ILog logger = new TacoLogger();
    const string csvPath = "TacoBell-US-AL.csv";
    static void Main(string[] args)
    {
        // Load the .env file
        string filepath = "secret.env";
        Env.Load(@"../../../" + filepath);

        try
        {
            WebScraper_GetAddress.RunWebScraper().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        // Objective: Find the two Taco Bells that are the farthest apart from one another. 

        //logger.LogInfo("Log initialized");
        string[] lines = File.ReadAllLines(@"CSV_Files/" + csvPath);
        if (lines.Length == 0)
        {
            logger.LogError("No lines found in csv file.");
        }
        if (lines.Length == 1)
        {
            logger.LogWarning("Only one line found in csv file.");
        }

        // This will display the first item in your lines array
        //logger.LogInfo($"Lines: {lines[0]}");

        // Create a new instance of your TacoParser class
        TacoParser parser = new TacoParser();

        // Use the Select LINQ method to parse every line in lines collection
        ITrackable[] locations = lines.Select(parser.Parse).ToArray();


        // Complete the Parse method in TacoParser class first and then START BELOW ----------

        // TODO: Create two `ITrackable` variables with initial values of `null`. 
        // These will be used to store your two Taco Bells that are the farthest from each other.
        ITrackable locA = null;
        ITrackable locB = null;
        // TODO: Create a `double` variable to store the distance
        double locDistance = 0;
        // TODO: Add the Geolocation library to enable location comparisons: using GeoCoordinatePortable;
        // Look up what methods you have access to within this library.

        // NESTED LOOPS SECTION----------------------------

        // FIRST FOR LOOP -
        // TODO: Create a loop to go through each item in your collection of locations.
        // This loop will let you select one location at a time to act as the "starting point" or "origin" location.
        // Naming suggestion for variable: `locA`

        // TODO: Once you have locA, create a new Coordinate object called `corA` with your locA's latitude and longitude.

        // SECOND FOR LOOP -
        // TODO: Now, Inside the scope of your first loop, create another loop to iterate through locations again.
        // This allows you to pick a "destination" location for each "origin" location from the first loop. 
        // Naming suggestion for variable: `locB`

        // TODO: Once you have locB, create a new Coordinate object called `corB` with your locB's latitude and longitude.

        // TODO: Now, still being inside the scope of the second for loop, compare the two locations using `.GetDistanceTo()` method, which returns a double.
        // If the distance is greater than the currently saved distance, update the distance variable and the two `ITrackable` variables you set above.

        for (int i = 0; i < locations.Length; i++)
        {
            var corA = new GeoCoordinate(locations[i].Location.Latitude, locations[i].Location.Longitude);
            for (int j = i + 1; j < locations.Length; j++)
            {
                var corB = new GeoCoordinate(locations[j].Location.Latitude, locations[j].Location.Longitude);
                if (locDistance < corA.GetDistanceTo(corB))
                {
                    locDistance = corA.GetDistanceTo(corB);
                    locA = locations[i];
                    locB = locations[j];
                }
            }
        }

        // NESTED LOOPS SECTION COMPLETE ---------------------

        // Once you've looped through everything, you've found the two Taco Bells farthest away from each other.
        // Display these two Taco Bell locations to the console.

        Console.WriteLine($"The two tacobells with the furthest distance are:\nName A: {locA.Name}\tLatitude: {locA.Location.Latitude}\tLongitude: {locA.Location.Longitude}\nName B: {locB.Name}\tLatitude: {locB.Location.Latitude}\tLongitude: {locB.Location.Longitude}");
    }
}
