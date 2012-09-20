using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskOptimizer.API;

namespace TaskOptimizer.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Coordinate> s = GetCSV(new FileInfo(@"C:\LatLon.csv"));
            DateTime startTime = DateTime.Now;
            Console.WriteLine(Precomp.getDistance(s[0], s[1]));

            var stops = new List<Coordinate>();
            var r = new Random();
            while (stops.Count < 10)
            {
                Coordinate c = s[r.Next(s.Count)];
                if (!stops.Contains(c))
                {
                    stops.Add(c);
                }
            }

            OSMResponse route = Precomp.getRoute(stops);
            foreach (OSMInstruction inst in route.Route_Instructions)
            {
                Console.WriteLine(inst.Road_Name + " " + inst.Instruction_Duration);
            }
            Console.WriteLine(DateTime.Now.Subtract(startTime).Ticks/10000000.00);

            Console.WriteLine(route.Route_Geometry);
        }

        private static void SpeedTest(int max, int iterations)
        {
            List<Coordinate> s = GetCSV(new FileInfo(@"C:\LatLon.csv"));
            int numStops;
            for (numStops = 5; numStops <= max; numStops++)
            {
                double avgtime = 0.0;
                for (int i = 0; i < iterations; i++)
                {
                    var stops = new List<Coordinate>();
                    var r = new Random();
                    while (stops.Count < numStops)
                    {
                        Coordinate c = s[r.Next(s.Count)];
                        if (!stops.Contains(c))
                        {
                            stops.Add(c);
                        }
                    }
                    DateTime nowTime = DateTime.Now;
                    Precomp.getRoute(stops);
                    avgtime += DateTime.Now.Subtract(nowTime).Ticks/10000000;
                }
                avgtime /= iterations;
                Console.WriteLine("Stops: " + numStops + " | Time: " + avgtime);
            }
        }

        /// <summary>
        /// Loads a set of points through CSV
        /// </summary>
        public static List<Coordinate> GetCSV(FileInfo f)
        {
            var coords = new List<Coordinate>();
            StreamReader fs = f.OpenText();
            while (!fs.EndOfStream)
            {
                string s = fs.ReadLine();
                string[] points = s.Split(new[] {','}, 2);
                points[1] = points[1].Replace(",", "");
                var c = new Coordinate(double.Parse(points[0]), double.Parse(points[1]));
                if (!coords.Contains(c)) coords.Add(c);
            }
            return coords;
        }
    }
}