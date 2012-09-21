using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskSequencer : Population<TaskSequence>
    {
        private readonly int _maxPopulationSize = 25;
        private readonly TaskSequencerNearestInsert _nearestInsert = new TaskSequencerNearestInsert();
        private FitnessLevels _fitnessLevels;
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
                else
                {
                    return bestIndividual;
                }
            }
        }

        public void configure(Configuration config)
        {
            if (!config.OrderedTasks && !isConfigurationChanged(config))
            {
                return;
            }

            _fitnessLevels = config.FitnessLevels;
            _worker = config.Worker;
            _tasks = new List<Task>(config.Tasks);

            configureMutationParameters();
            configureTaskUserIds();

            bool keepBest = false;

            if (config.OrderedTasks)
            {
                if (bestIndividual == null)
                {
                    var sequence = new TaskSequence();
                    sequence.Id = 0;
                    sequence.TaskSequencer = this;

                    bestIndividual = sequence;
                    Individuals[0] = bestIndividual;
                }


                bestIndividual.Worker = _worker;
                bestIndividual.FitnessLevels = _fitnessLevels;
                bestIndividual.Tasks = new List<Task>(_tasks);

                keepBest = true;
            }


            regeneratePopulation(keepBest);
        }

        public void useOptimalPopulationSize()
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

        private bool isConfigurationChanged(Configuration config)
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

        private void configureMutationParameters()
        {
            InitialMutationRate = (int) (2.5*computeOptimalPopulationSize(_tasks.Count));
            MutationRate = InitialMutationRate;
            InitialCataclysmCountdown = InitialMutationRate*10;
            CataclysmCountdown = InitialCataclysmCountdown;
        }

        private void configureTaskUserIds()
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
                    Individuals[t] = generateInitialSequenceUsingCheapestInsert();
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

            computeMinSequence();

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

        private void computeMinSequence()
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

        public void recomputeFitness()
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

        public List<Task> getOriginalTasks()
        {
            return _tasks;
        }

        private TaskSequence generateInitialSequenceUsingCheapestInsert()
        {
            TaskSequence sequence = _nearestInsert.Generate(new List<Task>(_tasks), _tasks.Count, _worker, _fitnessLevels);
            sequence.TaskSequencer = this;
            return sequence;
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public int ExpectedFitness { get; set; }
            public FitnessLevels FitnessLevels { get; set; }
            
            public bool OrderedTasks = false;

            public List<Task> Tasks { get; set; }
            public Worker Worker { get; set; }
        }

        #endregion
    }
}