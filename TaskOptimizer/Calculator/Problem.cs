using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TaskOptimizer.API;
using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public class Problem
    {
        //private static readonly PooledRedisClientManager rc = new PooledRedisClientManager();

        public static string getRawRoute(List<Coordinate> stops)
        {
            var resolved = new List<Coordinate>();
            foreach (Coordinate c in stops)
            {
                Coordinate r = OSRM.FindNearest(c);
                if (!resolved.Contains(r))
                {
                    resolved.Add(r);
                }
            }

            var stopTasks = new List<Task>();
            foreach (Coordinate r in resolved)
            {
                var t = new Task(resolved.IndexOf(r), resolved.Count);
                t.lat = r.lat;
                t.lon = r.lon;
                t.X = r.lat;
                t.Y = r.lon;
                t.Effort = 0;
                stopTasks.Add(t);
            }
            var optConf = new Optimizer.Configuration {tasks = stopTasks};
            var truck = new Robot();
            optConf.robots = new List<Robot> {truck};
            optConf.randomSeed = 777777;

            var fl = new FitnessLevels {CostMultiplier = 1, TimeMultiplier = 1};
            optConf.fitnessLevels = fl;
            optConf.startX = optConf.tasks[0].lat;
            optConf.startY = optConf.tasks[0].lon;
            optConf.nbDistributors = Environment.ProcessorCount*3;
            var o = new Optimizer(optConf);
            while (o.m_minDistributor.m_nbIterationsWithoutImprovements < 10000)
            {
            }

            var routeList = new List<Coordinate>();

            foreach (Task t in o.MinSequences[0].Tasks)
            {
                var rp = new Coordinate(t.X, t.Y);
                routeList.Add(rp);
            }
            
            return OSRM.CalculateRouteRaw(routeList);
        }

        public static String getMultiRoute(ICollection<Coordinate> coords, int numTrucks)
        {
            var resolved = new List<Coordinate>();
            foreach (Coordinate c in coords)
            {
                Coordinate r = OSRM.FindNearest(c);
                if (!resolved.Contains(r))
                {
                    resolved.Add(r);
                }
            }
            var stopTasks = new List<Task>();
            foreach (Coordinate r in resolved)
            {
                var t = new Task(resolved.IndexOf(r), resolved.Count);
                t.lat = r.lat;
                t.lon = r.lon;
                t.X = r.lat;
                t.Y = r.lon;
                t.Effort = 0;
                stopTasks.Add(t);
            }
            var optConf = new Optimizer.Configuration();
            optConf.tasks = stopTasks;
            var truck = new Robot();
            optConf.robots = new List<Robot>();
            for (int t = 0; t < numTrucks; t++)
            {
                optConf.robots.Add(truck);
            }
            optConf.randomSeed = 777777;
            var fl = new FitnessLevels();
            fl.CostMultiplier = 1;
            fl.TimeMultiplier = 100;
            optConf.fitnessLevels = fl;
            optConf.startX = optConf.tasks[0].lat;
            optConf.startY = optConf.tasks[0].lon;
            optConf.nbDistributors = Environment.ProcessorCount*3;
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