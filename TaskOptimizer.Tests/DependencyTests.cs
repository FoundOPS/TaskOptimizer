using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;

namespace TaskOptimizer.Tests
{
    /// <summary>
    /// Test the connection to dependencies
    /// </summary>
    [TestClass]
    public class DependencyTests
    {
        public DependencyTests()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        /// <summary>
        /// Calculate a 10 coordinate route with OSRM
        /// </summary>
        [TestMethod]
        public void OSRMCalculateRoute()
        {
            var stops = Tools.GetCoordinates(3);

            DateTime startTime = DateTime.Now;
            OSMResponse route = Osrm.GetInstance(new Guid()).CalculateRoute(stops);

            Trace.WriteLine(String.Format("Total Seconds {0}", DateTime.Now.Subtract(startTime).TotalSeconds));
            //foreach (OSMInstruction inst in route.Route_Instructions)
            //    Trace.WriteLine(inst.Road_Name + " " + inst.Instruction_Duration);

            Trace.WriteLine(route.Route_Geometry);
        }

        /// <summary>
        /// Find 10 coordinates with OSRM
        /// </summary>
        [TestMethod]
        public void OSRMFindCoordinates()
        {
            var stops = Tools.GetCoordinates();
            DateTime startTime = DateTime.Now;
            foreach (var stop in stops)
            {
                Osrm.GetInstance(new Guid()).FindNearest(stop);
            }

            Trace.WriteLine(String.Format("Total Seconds {0}", DateTime.Now.Subtract(startTime).TotalSeconds));
        }

        //private static void SpeedTest(int max, int iterations)
        //{
        //    List<Coordinate> s = Tools.GetCSVCoordinates();
        //    int numStops;
        //    for (numStops = 5; numStops <= max; numStops++)
        //    {
        //        double avgtime = 0.0;
        //        for (int i = 0; i < iterations; i++)
        //        {
        //            var stops = new List<Coordinate>();
        //            var r = new Random();
        //            while (stops.Count < numStops)
        //            {
        //                Coordinate c = s[r.Next(s.Count)];
        //                if (!stops.Contains(c))
        //                {
        //                    stops.Add(c);
        //                }
        //            }
        //            DateTime nowTime = DateTime.Now;
        //            OSMResponse route = OSRM.CalculateRoute(stops);
        //            avgtime += DateTime.Now.Subtract(nowTime).Ticks / 10000000.0;
        //        }
        //        avgtime /= iterations;
        //        Trace.WriteLine("Stops: " + numStops + " | Time: " + avgtime);
        //    }
        //}
    }
}