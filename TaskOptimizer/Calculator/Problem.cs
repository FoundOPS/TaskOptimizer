using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaskOptimizer.API;
using TaskOptimizer.Model;
using TaskOptimizer.Logging;

namespace TaskOptimizer.Calculator
{
    /// <summary>
    /// Representation of a routing problem.
    /// </summary>
    public class Problem
    {
        private readonly int _numberIterationsWithoutImprovement;   // Number of iterations to run (?)
        private int[,] _mCachedCosts;   // cache of task-to-task costs

        /// <summary>
        /// Instance of OSRM associated with this problem
        /// </summary>
        public Osrm Osrm { get; private set; }
        /// <summary>
        /// Instance of ICostFunction that calculates task-to-task cost
        /// </summary>
        public ICostFunction CostFunction { get; private set; }
        /// <summary>
        /// Raw data of task-to-task cost cache
        /// </summary>
        public Int32[,] CachedCosts { get { return _mCachedCosts; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="costFunction">Instance of problem-specific implementation of ICostFunction</param>
        /// <param name="numberIterationsWithoutImprovement">Number of iterations to run</param>
        public Problem(ICostFunction costFunction, int numberIterationsWithoutImprovement)
        {
            _numberIterationsWithoutImprovement = numberIterationsWithoutImprovement;
            CostFunction = costFunction;
        }

        /// <summary>
        /// Gets cached cost between 2 tasks
        /// </summary>
        /// <param name="id1">ID of the first task</param>
        /// <param name="id2">ID of the second task</param>
        /// <returns>Cost if exists; otherwise 0</returns>
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
        /// <summary>
        /// Caches the cost between 2 tasks
        /// </summary>
        /// <param name="id1">ID of the first task</param>
        /// <param name="id2">ID of the second task</param>
        /// <param name="cost">Calculated cost between these 2 tasks</param>
        /// <returns>Cost</returns>
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
        /// Attaches a logger to this problem.
        /// </summary>
        /// <param name="logger">Logger to attach</param>
        public void AttachLogger(TaskOptimizer.Logging.Logger logger)
        { this.OnLogMessage += logger.HandleMessage; }
        /// <summary>
        /// Detaches the specified logger so that it does not receive messages about this problem
        /// </summary>
        /// <param name="logger">Logger to detach</param>
        public void DetachLogger(TaskOptimizer.Logging.Logger logger)
        { this.OnLogMessage -= logger.HandleMessage; }

        // Logger events
        private event EventHandler<LoggerEventArgs> OnLogMessage
        {
            add { _logMessageHandler += value; }
            remove { _logMessageHandler -= value; }
        }
        private EventHandler<LoggerEventArgs> _logMessageHandler;
        public void SendLogMessage(String tag, String format, params Object[] args)
        {
            if (_logMessageHandler != null)
                _logMessageHandler(this, new LoggerEventArgs(tag, format, args));
        }


        /// <summary>
        /// Calculate the problem for a set of tasks
        /// </summary>
        /// <param name="tasksToCalculate">The tasks to calculate</param>
        /// <param name="trucks">The number of trucks</param>
        public String[] Calculate(IEnumerable<Task> tasksToCalculate, int trucks)
        {
            var tasks = tasksToCalculate.ToList();

            SendLogMessage("Problem.Calculate", "Starting calculation: Task Count = {0}; Truck Count = {1}; Iterations = {2}",
                tasks.Count, trucks, _numberIterationsWithoutImprovement);

            Osrm = new Osrm();

            SendLogMessage("Problem.Calculate", "Resolving input to OSRM nodes");
            //resolve each point on the map
            foreach (var t in tasks)
            {
                var r = Osrm.FindNearest(t.Coordinate);
                t.Coordinate = r;
            }

            SendLogMessage("Problem.Calculate", "Coordinate resolution complete!");

            if (Osrm == null) Osrm = new Osrm();

            _mCachedCosts = new int[tasks.Count(), tasks.Count()];
            var optConf = new Optimizer.Configuration { Tasks = tasks };

            var truck = new Worker();
            optConf.Workers = new List<Worker>();
            for (int t = 0; t < trucks; t++)
                optConf.Workers.Add(truck);

            optConf.RandomSeed = 777777;
            optConf.NumberDistributors = Environment.ProcessorCount * 3;

            SendLogMessage("Problem.Calculate", "Starting to create Optimizer instance..");
            var optimizer = new Optimizer(this, optConf);
            SendLogMessage("Problem.Calculate", "Finished creating Optimizer instnce!");
            Int32 i = 0;

            Int32 prevFitness = Int32.MaxValue; // Fitness of the previous iteration
            Int32 noImprovementCount = 0; // Number of iterations without fitness improvement

            //while (_optimizer.MinDistributor.NbIterationsWithoutImprovements < _numberIterationsWithoutImprovement)
            while (noImprovementCount < _numberIterationsWithoutImprovement)
            {
                optimizer.Compute();
                optimizer.RecomputeFitness();

                if (optimizer.Fitness >= prevFitness)
                    noImprovementCount++;
                else
                {
                    prevFitness = optimizer.Fitness;
                    noImprovementCount = 0;
                }

                SendLogMessage("FitnessImprovement", "Iteration = {0}; Fitness = {1}", i++, optimizer.Fitness); 
            }
            
            optimizer.Stop();
            
            // Write the message to console and logs
            SendLogMessage("TaskOptimizer", "Total Fitness = {0}", optimizer.Fitness);

            string response = "{";
            int cont = 0;
            for (int r = 0; r < optimizer.MinSequences.Count; r++)
            {
                if (optimizer.MinSequences[r] == null)
                {
                    cont++;
                    continue;
                }
                SendLogMessage("TaskOptimizer", "MinSequence[r].Tasks.Count = {0}", optimizer.MinSequences[r].Tasks.Count);
                SendLogMessage("TaskOptimizer", "MinSequence[r].Fitness = {0}", optimizer.MinSequences[r].Fitness);
                response += "\"" + ((r + 1) - cont) + "\"" + ": ";

                var routeList = optimizer.MinSequences[r].Tasks.Select(t => t.Coordinate).ToList();

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

            return new[] { response, optimizer.Fitness.ToString() };
        }
    }
}