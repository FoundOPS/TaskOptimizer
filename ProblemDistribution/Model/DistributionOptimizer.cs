using System;
using ProblemLib.DataModel;
using ProblemLib.API;
using ProblemLib.Interfaces;
using ProblemLib.DataModel;
using System.Collections.Generic;
using ProblemLib.Logging;
using ProblemLib.Preprocessing;
using ProblemLib;

namespace ProblemDistribution.Model
{
    /// <summary>
    /// Server side version of the TaskOptimizer.TaskDistributor
    /// </summary>
    /// <remarks>
    /// Due to multithreaded nature of the server software, nearly all previously statis
    /// stuff is now exposed via this class.
    /// </remarks>
    class DistributionOptimizer : Population<TaskDistribution>
    {
        private DistributionServer server;
        private String osrmAddress;
        private PreprocessedDataCache cache;
        private List<Worker> workers;
        private List<Task> tasks;

        private Boolean isInitialized = false;
        private Boolean isOptimizingSequences;
        private Int32 seqOptimizationIterations;

        #region Preprocessing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <param name="progressCallback"></param>
        /// <remarks>
        /// progressCallback accepts 2 arguments. 1st is operation code. 2nd is current iteration index. 
        /// progressCallback returns true if controller requests termination of preprocessing progress
        /// </remarks>
        public void PreprocessProblemData(Int64 start, Int32 len, Func<UInt16, Int32, Int32, Boolean> progressCallback)
        {
            // update existing cache
            cache.Factory.PullFromStore(cache, CacheType.NearestNode);
            cache.Factory.PullFromStore(cache, CacheType.DistanceTime);

            
            if (start > 0 && len > 0)
            {   // preprocess task-task data
                // create preprocessing partition
                PreprocessingPartition<Task> partition = new PreprocessingPartition<Task>(tasks.ToArray(), start, len);

                // precalculate progress reporting intervals
                int interval = partition.Size / 100;

                int counter = 0;
                int icounter = 0;
                foreach (Pair<Task, Task> p in partition)
                {
                    if (!cache.IsIstanceTimeEntryCached(p.First.Coordinates, p.Second.Coordinates))
                    {   // preprocess only if not found
                        // get dt pair from uncached redis api
                        Pair<Int32, Int32> dt = OsrmAPI.GetDistanceTime(osrmAddress, p.First.Coordinates, p.Second.Coordinates);
                        // add to cache
                        cache.AddCachedDistanceTimeEntry(p.First.Coordinates, p.Second.Coordinates, dt);
                    }

                    counter++;
                    icounter++;

                    // report progress if necessary
                    if (icounter >= interval)
                    {
                        icounter = 0;
                        if (progressCallback(ControlCodes.Preprocessing, counter, partition.Size))
                            return;
                    }
                }

                // wait for permission to upload data
                if (progressCallback(ControlCodes.WaitingForSync, counter, partition.Size))
                    return;

                // upload data
                try
                {
                    cache.Factory.PushToStore(cache, CacheType.DistanceTime);
                    progressCallback(ControlCodes.Acknowledge, counter, partition.Size);   // upload success, acknowledge
                }
                catch (Exception x)
                {
                    throw new ProblemLib.ErrorHandling.ProblemLibException(ErrorCodes.RedisConnectionFailed, x);
                }

            }
            else if (start == -1 && len == -1)
            {   // preprocess nearest-node data
                Task[] taskArray = tasks.ToArray();

                // precalculate progress reporting intervals
                int interval = taskArray.Length / 100;

                int counter = 0;
                for (int i = 0; i < taskArray.Length; i++, counter++)
                {
                    Coordinate current = taskArray[i].Coordinates;
                    if (!cache.IsNearestNodeEntryCached(current))
                    {
                        Coordinate nearestNode = OsrmAPI.FindNearestNode(osrmAddress, current);
                        cache.AddCachedNearestNodeEntry(current, nearestNode);
                    }

                    // report progress every {interval} iterations
                    if (counter >= interval)
                    {
                        counter = 0;
                        if (progressCallback(ControlCodes.Preprocessing, i, taskArray.Length)) return;
                    }
                }

                // report final progress and wait for permission to sync
                if (progressCallback(ControlCodes.WaitingForSync, taskArray.Length, taskArray.Length)) return;

                // upload data
                try
                {
                    cache.Factory.PushToStore(cache, CacheType.NearestNode);
                    progressCallback(ControlCodes.Acknowledge, taskArray.Length, taskArray.Length);   // upload success, acknowledge
                }
                catch (Exception x)
                {
                    throw new ProblemLib.ErrorHandling.ProblemLibException(ErrorCodes.RedisConnectionFailed, x);
                }

            }
            
        }

        #endregion

        #region Optimizing

        /// <summary>
        /// Cost function used by this optimizer instance
        /// </summary>
        public ICostFunction CostFunction { get; set; }

        public Int32 MutationForce
        {
            get
            {
                return (Int32)((InitialMutationRate - MutationRate) / (Double)InitialMutationRate * 10);
            }
        }

        public Int32 MaxMutations
        {
            get { return 0; }
        }


        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server that owns this optimizer thread</param>
        public DistributionOptimizer(DistributionServer server)
        {
            this.server = server;
        }
        /// <summary>
        /// Initializes the optimizer with specified configuration data
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(DistributionConfiguration config)
        {
            Configuration = config;

            // set up configuration
            Rand = new Random(config.RandomSeed);
            workers = new List<Worker>(config.Workers);
            tasks = new List<Task>(config.Tasks);
            
            // create osrm instance
            String redisAddress = String.Format("{0}:{1}", config.RedisServer, config.RedisServerPort);
            osrmAddress = String.Format("http://{0}:{1}/", config.OsrmServer, config.OsrmServerPort);
            
            // create cache
            RedisDataCacheFactory redisFactory = new RedisDataCacheFactory(config.RedisServer.ToString(), config.RedisServerPort, config.ProblemID);
            cache = redisFactory.CreateCache();

            // configure population size
            int problemComplexity = tasks.Count * workers.Count;
            _populationSize = (int)(10 * Math.Log10(problemComplexity) / Math.Log10(Math.E));

            InitialMutationRate = (int)(1.5 * _populationSize);
            MutationRate = InitialMutationRate;
            InitialCataclysmCountdown = InitialMutationRate * 10;
            CataclysmCountdown = InitialCataclysmCountdown;

            if (workers.Count == 1)
            { // No need for distribution if there is only one worker
                _populationSize = 1;
                MaxIterationsWithoutImprovements = -1;
            }

            // configure distributions
            Individuals = new TaskDistribution[_populationSize];
            for (int i = 0; i < Individuals.Length; i++)
            {
                // create and initialize task distribution
                Individuals[i] = new TaskDistribution(this) { Id = i };
                Individuals[i].Initialize(workers, tasks, Rand.Next());

                // generate initial solution: fixed for first few and randomized for the rest
                Individuals[i].GenerateInitialSolution(i < workers.Count ? i : -1);
            }

            // first update of the fitness
            UpdateFitness();

            isInitialized = true;
        }
        /// <summary>
        /// Causes this distribution and all its children to reevaluate their fitness values.
        /// </summary>
        public void UpdateFitness()
        {
            Int32 bestFitness = Int32.MaxValue;
            TaskDistribution bestDistribution = null;

            foreach (TaskDistribution distribution in Individuals)
            {
                distribution.UpdateFitness(); // update fitness value of the child
                if (distribution.Fitness < bestFitness) // if new best fitness found...
                {
                    bestDistribution = distribution;
                    bestFitness = distribution.Fitness;
                }
            }

        }

        #endregion

        /// <summary>
        /// Configuration used for initializing this instance
        /// </summary>
        public DistributionConfiguration Configuration { get; private set; }
        /// <summary>
        /// Returns a boolean indicating whether the optimizer object has been
        /// initialized.
        /// </summary>
        public Boolean IsInitialized
        { get { return isInitialized; } }


        #region Members of Population<T>

        /// <summary>
        /// Generates a new population while preserving best individuals
        /// </summary>
        protected override void RegeneratePopulation()
        {
            TaskDistribution best = null;
            int bestFitness = Int32.MaxValue;

            for (int i = 0; i < _populationSize; i++)
            {
                if (Individuals[i].Id != best.Id)
                {
                    if (i != 0)
                        Individuals[i].GenerateInitialSolution();
                    // TODO grab another good solution from server ??
                }

                if (Individuals[i].Fitness < bestFitness)
                {
                    bestFitness = Individuals[i].Fitness;
                    best = Individuals[i];
                }
            }

            OnNewBestIndividual(best);
        }
        /// <summary>
        /// Called when a new best individual is detected
        /// </summary>
        /// <param name="individual">The best individual</param>
        protected override void OnNewBestIndividual(TaskDistribution individual)
        {
            if (bestIndividual != null && bestIndividual.Fitness == individual.Fitness)
                return; // nothing changed

            TaskDistribution copy = (TaskDistribution)individual.Clone();

#if DEBUG // idk why there is such a check but i'll keep it here for now
            if (copy.Fitness != individual.Fitness)
                throw new Exception("Fitness mismatch when cloning task distribution");
#endif

            bestIndividual = copy;
            base.OnNewBestIndividual(individual);
        }
        /// <summary>
        /// Run one iteration of optimization
        /// </summary>
        public override void Optimize()
        {
            // compute optimization parameters
            if (!isOptimizingSequences && NbIterationsWithoutImprovements > MaxIterationsWithoutImprovements)
            {
                // target reached, stop optimizing distribution and switch to optimizing sequences of the best distribution
                bestIndividual.OptimizeSequences = true;
                seqOptimizationIterations = 0;

                isOptimizingSequences = true;
                seqOptimizationIterations++;
            }
            else if (workers.Count > 1 && isOptimizingSequences &&
                NbIterationsWithoutImprovements > 2 * MaxIterationsWithoutImprovements)
            {
                // restart distribution optimization
                foreach (TaskDistribution distribution in Individuals)
                    distribution.OptimizeSequences = false;

                NbIterationsWithoutImprovements = 0;
                isOptimizingSequences = false;
            }

            // run optimization
#if DEBUG   // performance logging only for debug build
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif

            base.Optimize();

            if (isOptimizingSequences)
                OnNewBestIndividual(bestIndividual);

#if DEBUG
            // Log execution time in debug mode
            GlobalLogger.SendLogMessage("Performance", "DistributionOptimizer.Optimize(): {0}ms", sw.ElapsedMilliseconds);
#endif
        }

        #endregion
    }
}
