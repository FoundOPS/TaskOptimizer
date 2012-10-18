using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskDistributor : Population<TaskDistribution>
    {
        private TaskDistribution _bestIndividual;
        private bool _isOptimizingSequences;
        private int _nbSequenceOptimizationIterations;
        private Optimizer _optimizer;
        private List<Worker> _workers;
        private List<Task> _tasks;

        public TaskDistributor(Configuration config)
        {
            Configure(config);
        }

        public int CurrentIteration
        {
            get { return CurIteration; }
        }

        public List<TaskSequence> MinSequences
        {
            get { return _bestIndividual.Sequences; }
        }

        public int Fitness
        {
            get { return _bestIndividual.Fitness; }
        }

        public void RecomputeFitness()
        {
            int bestFitness = Int32.MaxValue;
            TaskDistribution bestDistribution = null;
            foreach (TaskDistribution distribution in Individuals)
            {
                distribution.RecomputeFitness();
                if (distribution.Fitness < bestFitness)
                {
                    bestDistribution = distribution;
                    bestFitness = distribution.Fitness;
                }
            }

            OnNewBestIndividual(bestDistribution);
        }

        public override void Optimize()
        {
            ComputeOptimizationMode();

            DateTime start = DateTime.Now;

            base.Optimize();

            if (_isOptimizingSequences)
            {
                // make sure to update the best copy if has changed...
                OnNewBestIndividual(bestIndividual);
            }

            DateTime end = DateTime.Now;
            TimeSpan elapsed = end - start;


            if (elapsed.TotalMilliseconds > 300)
            {
                int a = 0;
            }
        }

        protected override void OnNewBestIndividual(TaskDistribution individual)
        {
            if (_bestIndividual != null && _bestIndividual.Fitness == individual.Fitness)
            {
                // no change!
                return;
            }

            var copy = new TaskDistribution(individual);

            if (copy.Fitness != individual.Fitness)
            {
                throw new Exception("Fitness mismatch when cloning task distribution");
            }


            _bestIndividual = copy;
            base.OnNewBestIndividual(individual);
        }

        private void Configure(Configuration config)
        {
            Rand = new Random(config.RandomSeed);

            _tasks = config.Tasks;
            _workers = config.Workers;
            _optimizer = config.Optimizer;
            
            ConfigureTaskDistributions();
        }


        private void ConfigureTaskDistributions()
        {
            ConfigurePopulationSize();

            Individuals = new TaskDistribution[_populationSize];

            var taskDistributionConfiguration = new TaskDistribution.Configuration(_workers, _tasks, this, Rand.Next());

            for (int t = 0; t < _populationSize; t++)
            {
                Individuals[t] = new TaskDistribution(taskDistributionConfiguration) {Id = t};
                if (t < _workers.Count)
                {
                    Individuals[t].GenerateFixedInitialSolution(t);
                }
                else
                {
                    Individuals[t].GenerateInitialSolution();
                }
            }

            RecomputeFitness();
        }

        private void ConfigurePopulationSize()
        {
            int problemComplexity = _tasks.Count * _workers.Count;
            _populationSize = (int)(10 * Math.Log10(problemComplexity) / Math.Log10(Math.E));

            InitialMutationRate = (int)(1.5 * _populationSize);
            MutationRate = InitialMutationRate;
            InitialCataclysmCountdown = InitialMutationRate * 10;
            CataclysmCountdown = InitialCataclysmCountdown;

            MaxIterationsWithoutImprovements = InitialCataclysmCountdown * 10;

            if (_workers.Count == 1)
            {
                // no need to distribute tasks!
                _populationSize = 1;
                MaxIterationsWithoutImprovements = -1;
            }
        }

        public int ComputeMutationForce()
        {
            return (int)((InitialMutationRate - MutationRate) / (double)InitialMutationRate * 10);
        }

        protected override int ComputeMaxMutations()
        {
            return 0;
        }

        protected override void RegeneratePopulation()
        {
            TaskDistribution bestIndividual = null;
            int bestFitness = Int32.MaxValue;

            for (int t = 0; t < _populationSize; t++)
            {
                // keep the best distribution...
                if (Individuals[t].Id != _bestIndividual.Id)
                {
                    if (t != 0)
                    {
                        Individuals[t].GenerateInitialSolution();
                    }
                    else
                    {
                        // import a solution from another population?
                        if (_optimizer.Fitness != Fitness && _optimizer.Fitness < Fitness * 1.1 && Rand.Next(2) == 0)
                        {
                            Individuals[t].SetSequences(_optimizer.MinSequences);
                        }
                    }
                }

                if (Individuals[t].Fitness < bestFitness)
                {
                    bestFitness = Individuals[t].Fitness;
                    bestIndividual = Individuals[t];
                }
            }

            OnNewBestIndividual(bestIndividual);
        }

        private void ComputeOptimizationMode()
        {
            if (!_isOptimizingSequences && NbIterationsWithoutImprovements > MaxIterationsWithoutImprovements)
            {
                // we've found a possible good distribution solution... so now, optimize the sequences
                bestIndividual.OptimizeSequences = true;
                _nbSequenceOptimizationIterations = 0;

                _isOptimizingSequences = true;
                _nbSequenceOptimizationIterations++;
            }
            else if (_workers.Count > 1 && _isOptimizingSequences &&
                     NbIterationsWithoutImprovements > 2 * MaxIterationsWithoutImprovements)
            {
                // distribution found! restart distribution optimization!
                foreach (TaskDistribution distribution in Individuals)
                {
                    distribution.OptimizeSequences = false;
                }
                NbIterationsWithoutImprovements = 0;
                _isOptimizingSequences = false;
            }
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public List<Task> Tasks { get; set; }
            public List<Worker> Workers { get; set; }

            public Optimizer Optimizer { get; set; }
            public int RandomSeed { get; set; }

            public int StartProgressPercent { get; set; }
            public int EndProgressPercent { get; set; }
        }

        #endregion
    }
}