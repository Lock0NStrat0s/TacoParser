using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

using LoggingKata;

public static class API_AddressToCoords
{
    public static async Task RunAPI()
    {
        string address = "5731 11A AVE NW, EDMONTON, AB";
        string apiKey = "8e4735d28ba84ce2b5bba5b838168162";

        try
        {
            var coordinates = await GetCoordinatesFromAddress(address, apiKey);
            Console.WriteLine($"Latitude: {coordinates.Item1}, Longitude: {coordinates.Item2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static async Task<(double, double)> GetCoordinatesFromAddress(string address, string apiKey)
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

                return (lat, lng);
            }
            else
            {
                throw new Exception("Geocoding failed: " + json["status"]["message"]);
            }
        }
    }
}