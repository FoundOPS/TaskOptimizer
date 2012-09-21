using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskSequencer : Population<TaskSequence>
    {
        private readonly TaskSequencerNearestInsert _nearestInsert = new TaskSequencerNearestInsert();

        private Worker _worker;
        private List<Task> _tasks;

        public TaskSequencer(int maxTasks)
        {
            _populationSize = 1;
            _maxPopulationSize = computeOptimalPopulationSize(maxTasks);
            Individuals = new TaskSequence[_maxPopulationSize];
        }

        public int PopulationSize
        {
            get { return _populationSize; }

            set
            {
                if (value > _maxPopulationSize)
                {
                    value = _maxPopulationSize;
                }

                if (value < 1)
                {
                    value = 1;
                }

                if (value == PopulationSize)
                {
                    // nothing to do!
                    return;
                }

                _populationSize = value;
                regeneratePopulation(true);
            }
        }
        private readonly int _maxPopulationSize = 25;
        public int MaxPopulationSize
        {
            get { return _maxPopulationSize; }
        }

        public TaskSequence MinTaskSequence
        {
            get
            {
                if (BestFitness == 0)
                {
                    return null;
                }

                return bestIndividual;
            }
        }

        public void Configure(Configuration config)
        {
            if (!config.OrderedTasks && !IsConfigurationChanged(config))
            {
                return;
            }

            _worker = config.Worker;
            _tasks = new List<Task>(config.Tasks);

            ConfigureMutationParameters();
            ConfigureTaskUserIds();

            bool keepBest = false;

            if (config.OrderedTasks)
            {
                if (bestIndividual == null)
                {
                    var sequence = new TaskSequence {Id = 0, TaskSequencer = this};

                    bestIndividual = sequence;
                    Individuals[0] = bestIndividual;
                }


                bestIndividual.Worker = _worker;
                bestIndividual.Tasks = new List<Task>(_tasks);

                keepBest = true;
            }


            regeneratePopulation(keepBest);
        }

        public void UseOptimalPopulationSize()
        {
            PopulationSize = computeOptimalPopulationSize(_tasks.Count);
        }

        public override void Optimize()
        {
            if (_tasks.Count == 0)
            {
                return;
            }

            base.Optimize();
        }

        private int computeOptimalPopulationSize(int nbTasks)
        {
            return (int) (Math.Log10(nbTasks)*10) + 1;
        }

        private bool IsConfigurationChanged(Configuration config)
        {
            if (bestIndividual != null && bestIndividual.Tasks.Count == config.Tasks.Count)
            {
                int t = 0;
                bool different = false;
                foreach (Task task in bestIndividual.Tasks)
                {
                    if (task.Id != (config.Tasks[t]).Id)
                    {
                        different = true;
                        break;
                    }
                    t++;
                }

                if (!different)
                {
                    // all tasks are the same...no change!
                    return false;
                }
            }

            return true;
        }

        private void ConfigureMutationParameters()
        {
            InitialMutationRate = (int) (2.5*computeOptimalPopulationSize(_tasks.Count));
            MutationRate = InitialMutationRate;
            InitialCataclysmCountdown = InitialMutationRate*10;
            CataclysmCountdown = InitialCataclysmCountdown;
        }

        private void ConfigureTaskUserIds()
        {
            for (int t = 0; t < _tasks.Count; t++)
            {
                _tasks[t].UserId = t;
            }
        }

        /// <summary>
        /// Regenerate new sequences but keep the current best
        /// </summary>
        private void regeneratePopulation(bool keepBest)
        {
            int bestFitness = Int32.MaxValue;

            if (bestIndividual != null)
            {
                bestFitness = bestIndividual.Fitness;
            }

            if (!keepBest)
            {
                bestIndividual = null;
                BestFitness = Int32.MaxValue;
            }

            int firstIndex = 0;
            if (keepBest && bestIndividual != null && bestIndividual.Id >= _populationSize)
            {
                firstIndex = 1;
                Individuals[0] = bestIndividual;
                bestIndividual.Id = 0;
            }


            for (int t = firstIndex; t < _populationSize; t++)
            {
                if (Individuals[t] == null)
                {
                    Individuals[t] = GenerateInitialSequenceUsingCheapestInsert();
                }
                else
                {
                    if (keepBest && bestIndividual != null && Individuals[t].Id == bestIndividual.Id)
                    {
                    }
                    else
                    {
                        Individuals[t].Worker = _worker;
                        _nearestInsert.Generate(new List<Task>(_tasks), _tasks.Count, Individuals[t]);
                    }
                }
                Individuals[t].Id = t;
            }

            ComputeMinSequence();

            if (keepBest && BestFitness > bestFitness)
            {
                int a = 0;
            }
        }


        protected override void RegeneratePopulation()
        {
            regeneratePopulation(true);
        }


        protected override int ComputeMaxChildren()
        {
            return (int) (_populationSize*0.05 + 2);
        }

        protected override int ComputeMaxMutations()
        {
            return 0;
        }

        private void ComputeMinSequence()
        {
            int bestFitness = Int32.MaxValue;
            TaskSequence bestIndividual = null;

            for (int t = 0; t < _populationSize; t++)
            {
                if (Individuals[t] != null && Individuals[t].Fitness < bestFitness)
                {
                    bestFitness = Individuals[t].Fitness;
                    bestIndividual = Individuals[t];
                }
            }

            OnNewBestIndividual(bestIndividual);
        }

        public void RecomputeFitness()
        {
            int bestFitness = Int32.MaxValue;
            TaskSequence bestIndividual = null;

            for (int t = 0; t < _populationSize; t++)
            {
                if (Individuals[t] != null)
                {
                    Individuals[t].UpdateFitness();

                    if (Individuals[t].Fitness < bestFitness)
                    {
                        bestFitness = Individuals[t].Fitness;
                        bestIndividual = Individuals[t];
                    }
                }
            }

            OnNewBestIndividual(bestIndividual);
        }

        public List<Task> GetOriginalTasks()
        {
            return _tasks;
        }

        private TaskSequence GenerateInitialSequenceUsingCheapestInsert()
        {
            TaskSequence sequence = _nearestInsert.Generate(new List<Task>(_tasks), _tasks.Count, _worker);
            sequence.TaskSequencer = this;
            return sequence;
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public int ExpectedFitness { get; private set; }

            public bool OrderedTasks { get; private set; }

            public List<Task> Tasks { get; private set; }
            public Worker Worker { get; private set; }

            public Configuration(Worker worker, List<Task> tasks, int expectedFitness = 0, bool orderedTasks = false)
            {
                Worker = worker;
                Tasks = tasks;

                ExpectedFitness = expectedFitness;
                OrderedTasks = orderedTasks;
            }
        }

        #endregion
    }
}