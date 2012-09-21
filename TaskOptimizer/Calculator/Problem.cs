using System;
using System.Collections.Generic;
using ServiceStack.Redis;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        private static readonly PooledRedisClientManager RedisClientManager = new PooledRedisClientManager(Constants.RedisServer);

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
                var t = new Task(resolved.IndexOf(r), resolved.Count) { Lat = r.lat, Lon = r.lon, Effort = 0 };
                stopTasks.Add(t);
            }

            var optConf = new Optimizer.Configuration { Tasks = stopTasks };

            var truck = new Worker();
            optConf.Workers = new List<Worker>();
            for (int t = 0; t < trucks; t++)
            {
                optConf.Workers.Add(truck);
            }
            optConf.RandomSeed = 777777;

            optConf.FitnessLevels = new FitnessLevels { CostMultiplier = 1, TimeMultiplier = 100 };
            optConf.NumberDistributors = Environment.ProcessorCount * 3;

            var o = new Optimizer(optConf);
            while (o.MinDistributor.NbIterationsWithoutImprovements < 10000)
            {
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