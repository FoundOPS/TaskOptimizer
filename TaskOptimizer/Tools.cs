using System;
using System.Collections.Generic;
using System.IO;
using TaskOptimizer.API;

namespace TaskOptimizer.Tests
{
    public static class Tools
    {
        /// <summary>
        /// Loads the LatLon.csv coordinates
        /// </summary>
        public static List<Coordinate> GetCSVCoordinates()
        {
            var f = new FileInfo(Constants.RootDirectory + "LatLon.csv");
            var coords = new List<Coordinate>();
            StreamReader fs = f.OpenText();
            while (!fs.EndOfStream)
            {
                string s = fs.ReadLine();
                string[] points = s.Split(new[] { ',' }, 2);
                points[1] = points[1].Replace(",", "");
                var c = new Coordinate(double.Parse(points[0]), double.Parse(points[1]));
                if (!coords.Contains(c)) coords.Add(c);
            }
            return coords;
        }

        /// <summary>
        /// Get n random coordinates from the CSV
        /// </summary>
        /// <param name="number">The number of coordinates to load. Default = 10</param>
        public static List<Coordinate> GetCoordinates(int number = 10)
        {
            var csvCoordinates = GetCSVCoordinates();

            if (number > csvCoordinates.Count)
            {
                throw new Exception("Cannot get more coordinates than the CSV has!");
            }

            var coordinates = new List<Coordinate>();
            for (int i = 0; i < number; i++)
            {
                coordinates.Add(csvCoordinates[i]);
            }

            return coordinates;
        }
    }
}