using System;
using System.Collections.Generic;
using System.Threading;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        /// <summary>
        /// Calculates the best distribution/organization for a number of trucks and destinations
        /// </summary>
        /// <param name="destinations">The destinations to service</param>
        /// <param name="trucks">The number of trucks</param>
        /// <returns></returns>
        public static String Calculate(ICollection<Coordinate> destinations, int trucks)
        {
            var resolved = new List<Coordinate>();
            foreach (var c in destinations)
            {
                var r = OSRM.FindNearest(c);
                if (!resolved.Contains(r))
                    resolved.Add(r);
            }

            var stopTasks = new List<Task>();
            for (int i = 0; i < resolved.Count; i++ )
            {
                var c = resolved[i];
                var t = new Task(i, resolved.Count) { Lat = c.lat, Lon = c.lon, Effort = 0 };
                stopTasks.Add(t);
            }

            var optConf = new Optimizer.Configuration { Tasks = stopTasks };

            var truck = new Worker();
            optConf.Workers = new List<Worker>();
            for (int t = 0; t < 1; t++)
            {
                optConf.Workers.Add(truck);
            }
            optConf.RandomSeed = 777777;

            optConf.NumberDistributors = Environment.ProcessorCount * 3;

            var o = new Optimizer(optConf);
            //TODO figure out new stopping logic, this only works for 1 truck
            while (o.MinDistributor.NbIterationsWithoutImprovements < 10000)
            {
                Thread.Sleep(1000);
            }
            o.Stop();
            string response = "{";
            int cont = 0;
            for (int r = 0; r < o.MinSequences.Count; r++)
            {
                if (o.MinSequences[r] == null)
                {
                    cont++;
                    continue;
                }
                Console.WriteLine(o.MinSequences[r].Tasks.Count);
                response += "\"" + ((r + 1) - cont) + "\"" + ": ";
                var routeList = new List<Coordinate>();

                foreach (Task t in o.MinSequences[r].Tasks)
                {
                    var rp = new Coordinate(t.Lat, t.Lon);
                    routeList.Add(rp);
                }

                response += OSRM.CalculateRouteRaw(routeList) + ",";
            }
            response = response.Substring(0, response.Length - 1);
            return response + "}";
        }
    }
}