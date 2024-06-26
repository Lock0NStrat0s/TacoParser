using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace LoggingKata;

public static class WebScraper_GetAddress
{
    public static async Task RunWebScraper()
    {
        string url = "https://locations.tacobell.ca/en/ab/calgary";  // URL for Taco Bell locations in Alberta
        List<string> locations = new List<string>();
        try
        {
            locations = await GetTacoBellLocations(url);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        foreach (var location in locations)
        {
            Console.WriteLine(location);
        }
    }

    public static async Task<List<string>> GetTacoBellLocations(string url)
    {
        var locations = new List<string>();

        using (HttpClient client = new HttpClient())
        {
            // Fetch the HTML content of the page
            string response = null;
            try
            {
                response = await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            // Load the HTML content into HtmlAgilityPack
            HtmlDocument document = new HtmlDocument();
            try
            {
                document.LoadHtml(response);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            // Select the nodes that contain the location information
            HtmlNodeCollection locationNodes = null;
            try
            {
                locationNodes = document.DocumentNode.SelectNodes("//div[contains(@class, 'Address-line')]");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            if (locationNodes != null)
            {
                foreach (var node in locationNodes)
                {
                    // Extract the text content of each address node
                    var address = node.InnerText.Trim();
                    locations.Add(address);
                }
            }
            else
            {
                Console.WriteLine("No location nodes found.");
            }
        }

        return locations;
    }
}