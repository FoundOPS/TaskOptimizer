using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        private int[,] _mCachedCosts;

        public OSRM Osrm { get; private set; }
        public ICostFunction CostFunction { get; private set; }
        public Int32[,] CachedCosts { get { return _mCachedCosts; } }

        public Problem(ICostFunction costFunction)
        {
            CostFunction = costFunction;
        }

        public Int32 GetCachedCost(Int32 id1, Int32 id2)
        {
            if (id1 > id2)
            {
                Int32 tmp = id2;
                id2 = id1;
                id1 = tmp;
            }

            return _mCachedCosts[id1, id2];
        }
        public Int32 SetCachedCost(Int32 id1, Int32 id2, Int32 cost)
        {
            if (id1 > id2)
            {
                Int32 tmp = id2;
                id2 = id1;
                id1 = tmp;
            }

            _mCachedCosts[id1, id2] = cost;
            return cost;
        }

        public String Calculate(ICollection<Coordinate> destinations, int trucks)
        {
            Osrm = new OSRM();

            var resolved = new List<Coordinate>();
            //resolve each point on the map
            foreach (var c in destinations)
            {
                var r = Osrm.FindNearest(c);
                if (!resolved.Contains(r))
                    resolved.Add(r);
            }

            var stopTasks = new List<Task>();
            for (int i = 0; i < resolved.Count; i++)
            {
                var c = resolved[i];
                var t = new Task(i, resolved.Count) { Lat = c.lat, Lon = c.lon, Time = 30 * 60, Problem = this };
                stopTasks.Add(t);
            }

            return Calculate(stopTasks, trucks);
        }

        /// <summary>
        /// Calculates the best distribution/organization for a number of trucks and destinations
        /// </summary>
        /// <param name="trucks">The number of trucks</param>
        /// <returns></returns>
        public String Calculate(ICollection<Task> tasks, int trucks)
        {
            if (Osrm == null) Osrm = new OSRM();

            List<Task> lstTasks = new List<Task>(tasks);
            _mCachedCosts = new int[lstTasks.Count, lstTasks.Count];
            var optConf = new Optimizer.Configuration { Tasks = lstTasks };

            var truck = new Worker();
            optConf.Workers = new List<Worker>();
            for (int t = 0; t < trucks; t++)
                optConf.Workers.Add(truck);

            optConf.RandomSeed = 777777;
            optConf.NumberDistributors = Environment.ProcessorCount * 3;

            var o = new Optimizer(optConf);
            while (o.MinDistributor.NbIterationsWithoutImprovements < 10000)
            {
                o.Compute();
                //Thread.Sleep(1000); // TODO Deal with this!!
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

                if (routeList.Count > 1)
                {
                    var rawRoute = Osrm.CalculateRouteRaw(routeList);
                    Trace.WriteLine(rawRoute);
                    response += rawRoute + ",";
                }
                else
                {
                    //TODO (single stop routes cannot be calculated with OSRM)
                }
            }

            Osrm.Dispose();
            Osrm = null;

            response = response.Substring(0, response.Length - 1);
            return response + "}";

        }
    }
}