using System;
using System.Collections.Generic;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        //private static readonly PooledRedisClientManager rc = new PooledRedisClientManager();

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
            foreach (var r in resolved)
            {
                var t = new Task(resolved.IndexOf(r), resolved.Count) { lat = r.lat, lon = r.lon, X = r.lat, Y = r.lon, Effort = 0 };
                stopTasks.Add(t);
            }

            var optConf = new Optimizer.Configuration { tasks = stopTasks };

            var truck = new Robot();
            optConf.robots = new List<Robot>();
            for (int t = 0; t < trucks; t++)
            {
                optConf.robots.Add(truck);
            }
            optConf.randomSeed = 777777;

            optConf.fitnessLevels = new FitnessLevels { CostMultiplier = 1, TimeMultiplier = 100 };
            optConf.startX = optConf.tasks[0].lat;
            optConf.startY = optConf.tasks[0].lon;
            optConf.nbDistributors = Environment.ProcessorCount * 3;

            var o = new Optimizer(optConf);
            while (o.m_minDistributor.m_nbIterationsWithoutImprovements < 10000)
            {
            }
            o.stop();
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
                    var rp = new Coordinate(t.X, t.Y);
                    routeList.Add(rp);
                }

                response += OSRM.CalculateRouteRaw(routeList) + ",";
            }
            response = response.Substring(0, response.Length - 1);
            return response + "}";
        }
    }
}