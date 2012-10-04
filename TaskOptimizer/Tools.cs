using System;
using System.Collections.Generic;
using System.IO;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;
using TaskOptimizer.Model;

namespace TaskOptimizer
{
    public static class Tools
    {
        /// <summary>
        /// Get n coordinates from the CSV
        /// </summary>
        /// <param name="total">The number of coordinates to load. Defaults to 10</param>
        /// <param name="random">Choose random coordinates. Defaults to false</param>
        public static List<Coordinate> GetCoordinates(int total = 10, bool random = false)
        {
            var f = new FileInfo(Constants.RootDirectory + "LatLon.csv");
            var csvCoordinates = new List<Coordinate>();
            var fs = f.OpenText();
            while (!fs.EndOfStream)
            {
                string s = fs.ReadLine();
                string[] points = s.Split(new[] { ',' }, 2);
                points[1] = points[1].Replace(",", "");
                var c = new Coordinate(double.Parse(points[0]), double.Parse(points[1]));
                if (!csvCoordinates.Contains(c))
                    csvCoordinates.Add(c);
            }

            if (total > csvCoordinates.Count)
            {
                throw new Exception("Cannot get more coordinates than the CSV has!");
            }

            var coordinates = new List<Coordinate>();
            var r = new Random();
            for (var i = 0; i < total; i++)
            {
                Coordinate coordinate;

                if (random)
                {
                    do
                    {
                        coordinate = csvCoordinates[r.Next(csvCoordinates.Count)];
                    } while (!coordinates.Contains(coordinate));
                }
                else
                {
                    coordinate = csvCoordinates[i];
                }

                coordinates.Add(coordinate);
                csvCoordinates.Remove(coordinate);
            }

            return coordinates;
        }

        /// <summary>
        /// Create tasks for a problem from a set of coordinates
        /// </summary>
        public static List<Task> GetTasks(IEnumerable<Coordinate> coordinates, Problem problem)
        {
            var tasks = new List<Task>();
            int id = 0;
            foreach (var coordinate in coordinates)
            {
                var newTask = new Task(id, coordinate.lat, coordinate.lon) { Time = 30 * 60, Problem = problem };
                tasks.Add(newTask);
                id++;
            }

            return tasks;
        }
    }
}