using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

using LoggingKata;
using CsvHelper.Configuration;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LoggingKata.Models;
using DotNetEnv;

public static class API_AddressToCoords
{
    public static async Task<((double, double), string)> RunAPI(string address)
    {
        

        // Access the environment variables
        string apiKey = Environment.GetEnvironmentVariable("API_KEY");
        
        //Console.WriteLine(apiKey);


        ((double, double), string) coordinates = new();
        try
        {
            coordinates = await GetCoordinatesFromAddress(address, apiKey);
            Console.WriteLine($"Latitude: {coordinates.Item1.Item1}\tLongitude: {coordinates.Item1.Item2}\tAddress: {coordinates.Item2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return coordinates;
    }

    private static async Task<((double, double), string)> GetCoordinatesFromAddress(string address, string apiKey)
    {
        string url = $"https://api.opencagedata.com/geocode/v1/json?q={Uri.EscapeDataString(address)}&key={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json["status"]["code"].Value<int>() == 200)
            {
                var location = json["results"][0]["geometry"];
                double lat = location["lat"].Value<double>();
                double lng = location["lng"].Value<double>();

                var adrs = json["results"][0]["formatted"].Value<string>();

                return ((lat, lng), adrs);
            }
            else
            {
                throw new Exception("Geocoding failed: " + json["status"]["message"]);
            }
        }
    }
}