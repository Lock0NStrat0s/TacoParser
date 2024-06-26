﻿using System.Threading.Tasks;
using LoggingKata.Logger;
using LoggingKata.Models;

namespace LoggingKata;

/// <summary>
/// Parses a POI file to locate all the Taco Bells
/// </summary>
public class TacoParser
{
    readonly ILog logger = new TacoLogger();

    public ITrackable Parse(string line)
    {
        var cells = line.Split(',');

        if (cells.Length < 3)
        {
            logger.LogError("Array length is less than 3");
            return null;
        }

        string name = cells[2];
        double latitude = double.Parse(cells[0]);
        double longitude = double.Parse(cells[1]);

        Point point = new Point();
        point.Latitude = latitude;
        point.Longitude = longitude;

        TacoBellLocation tacoBell = new TacoBellLocation();
        tacoBell.Name = name;
        tacoBell.Location = point;

        return tacoBell;
    }
}
