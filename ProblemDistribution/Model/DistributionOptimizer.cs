using System;
using ProblemLib.DataModel;
using ProblemLib.API;
using ProblemLib.Interfaces;
using ProblemLib.DataModel;
using System.Collections.Generic;

namespace ProblemDistribution.Model
{
    class DistributionOptimizer : Population<TaskDistribution>
    {
        private DistributionServer server;
        private TaskDistribution bestIndividual;
        private List<Worker> workers;
        private List<Task> tasks;

        // remove this??
        public DistributionConfiguration Configuration { get; private set; }

        public Osrm Osrm { get; private set; }

        // Public

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
            String osrmAddress = String.Format("http://{0}:{1}/", config.OsrmServer, config.OsrmServerPort);
            Osrm = Osrm.GetInstance(config.ProblemID, redisAddress, osrmAddress);

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
                Individuals[i] = new TaskDistribution(this);
                Individuals[i].Initialize(workers, tasks, Rand.Next());

                // generate initial solution: fixed for first few and randomized for the rest
                Individuals[i].GenerateInitialSolution(i < workers.Count ? i : -1);
            }

            // first update of the fitness
            UpdateFitness();
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


        // Protected




        #region Members of Population<T>

        protected override void RegeneratePopulation()
        {
            throw new NotImplementedException();
        }

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

        public override void Optimize()
        {


            base.Optimize();
        }

        #endregion
    }
}
