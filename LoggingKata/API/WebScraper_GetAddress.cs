using CsvHelper.Configuration;
using CsvHelper;
using HtmlAgilityPack;
using LoggingKata.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoggingKata.API;

public static class WebScraper_GetAddress
{
    public static async Task RunWebScraper()
    {
        string url = "https://locations.tacobell.ca/en/ab/edmonton";
        List<string> locations = new List<string>();
        try
        {
            locations = await GetTacoBellLocations(url);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        List<TacoBellLocation> tbList = new List<TacoBellLocation>();
        foreach (var location in locations)
        {
            var coords = await API_AddressToCoords.RunAPI(location);
TacoBellLocation tbLoc = new TacoBellLocation() { Name = coords.Item2, Location = new Point() { Latitude = coords.Item1.Item1, Longitude = coords.Item1.Item2 } };
            tbList.Add(tbLoc);
        }


        WriteToCsv(@"../../../CSV_Files/TacoBellCanada.csv", tbList);

    }

    private static async Task<List<string>> GetTacoBellLocations(string url)
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
                bool flag = false;
                string temp = "";
                foreach (var node in locationNodes)
                {
                    // Extract the text content of each address node
                    var address = node.InnerText.Trim();

                    if (!flag)
                    {
                        temp += address;
                    }
                    else
                    {
                        temp += ", " + address;
                        locations.Add(temp);
                        temp = "";
                    }
                    flag = !flag;
                }
            }
            else
            {
                Console.WriteLine("No location nodes found.");
            }
        }

        return locations;
    }

    private static void WriteToCsv(string filePath, List<TacoBellLocation> data)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(data);
        }
    }
}