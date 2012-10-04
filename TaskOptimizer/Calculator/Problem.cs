using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        private readonly int _numberIterationsWithoutImprovement;
        private int[,] _mCachedCosts;

        public Osrm Osrm { get; private set; }
        public ICostFunction CostFunction { get; private set; }
        public Int32[,] CachedCosts { get { return _mCachedCosts; } }

        public Problem(ICostFunction costFunction, int numberIterationsWithoutImprovement)
        {
            _numberIterationsWithoutImprovement = numberIterationsWithoutImprovement;
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

        /// <summary>
        /// Calculate the problem for a set of tasks
        /// </summary>
        /// <param name="tasksToCalculate">The tasks to calculate</param>
        /// <param name="trucks">The number of trucks</param>
        public String[] Calculate(IEnumerable<Task> tasksToCalculate, int trucks)
        {
            var tasks = tasksToCalculate.ToList();

            Osrm = new Osrm();

            //resolve each point on the map
            foreach (var t in tasks)
            {
                var r = Osrm.FindNearest(t.Coordinate);
                t.Coordinate = r;
            }

            if (Osrm == null) Osrm = new Osrm();

            _mCachedCosts = new int[tasks.Count(), tasks.Count()];
            var optConf = new Optimizer.Configuration { Tasks = tasks };

            var truck = new Worker();
            optConf.Workers = new List<Worker>();
            for (int t = 0; t < trucks; t++)
                optConf.Workers.Add(truck);

            optConf.RandomSeed = 777777;
            optConf.NumberDistributors = Environment.ProcessorCount * 3;

            var o = new Optimizer(optConf);
            while (o.MinDistributor.NbIterationsWithoutImprovements < _numberIterationsWithoutImprovement)
            {
                o.Compute();
            }
            o.Stop();

            Console.WriteLine("Total Fitness " + o.Fitness);

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

                var routeList = o.MinSequences[r].Tasks.Select(t => t.Coordinate).ToList();

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

            response = response.Substring(0, response.Length - 1) + response + "}";

            return new[] { response, o.Fitness.ToString() };
        }
    }
}