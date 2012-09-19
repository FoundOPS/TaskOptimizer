using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskSequencer : Population<TaskSequence>
    {
        private readonly int m_maxPopulationSize = 25;
        private readonly TaskSequencerNearestInsert m_nearestInsert = new TaskSequencerNearestInsert();
        private FitnessLevels m_fitnessLevels;
        private int m_maxTasks;
        private Robot m_robot;
        private double m_startX, m_startY;
        private List<Task> m_tasks;

        public TaskSequencer(int maxTasks)
        {
            m_maxTasks = maxTasks;
            m_populationSize = 1;
            m_maxPopulationSize = computeOptimalPopulationSize(maxTasks);
            m_individuals = new TaskSequence[m_maxPopulationSize];
        }


        public int PopulationSize
        {
            get { return m_populationSize; }

            set
            {
                if (value > m_maxPopulationSize)
                {
                    value = m_maxPopulationSize;
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

                m_populationSize = value;
                regeneratePopulation(true);
            }
        }

        public int MaxPopulationSize
        {
            get { return m_maxPopulationSize; }
        }

        public TaskSequence MinTaskSequence
        {
            get
            {
                if (m_bestFitness == 0)
                {
                    return null;
                }
                else
                {
                    return m_bestIndividual;
                }
            }
        }

        public void configure(Configuration config)
        {
            if (!config.orderedTasks && !isConfigurationChanged(config))
            {
                return;
            }

            m_fitnessLevels = config.fitnessLevels;
            m_robot = config.robot;
            m_tasks = new List<Task>(config.tasks);
            m_startX = config.startX;
            m_startY = config.startY;

            configureMutationParameters();
            configureTaskUserIds();

            bool keepBest = false;

            if (config.orderedTasks)
            {
                if (m_bestIndividual == null)
                {
                    var sequence = new TaskSequence();
                    sequence.Id = 0;
                    sequence.TaskSequencer = this;

                    m_bestIndividual = sequence;
                    m_individuals[0] = m_bestIndividual;
                }


                m_bestIndividual.Robot = m_robot;
                m_bestIndividual.StartX = m_startX;
                m_bestIndividual.StartY = m_startY;
                m_bestIndividual.FitnessLevels = m_fitnessLevels;
                m_bestIndividual.Tasks = new List<Task>(m_tasks);

                keepBest = true;
            }


            regeneratePopulation(keepBest);
        }

        public void useOptimalPopulationSize()
        {
            PopulationSize = computeOptimalPopulationSize(m_tasks.Count);
        }


        public override void optimize()
        {
            if (m_tasks.Count == 0)
            {
                return;
            }

            base.optimize();
        }


        private int computeOptimalPopulationSize(int nbTasks)
        {
            return (int) (Math.Log10(nbTasks)*10) + 1;
        }


        private bool isConfigurationChanged(Configuration config)
        {
            if (m_bestIndividual != null && m_bestIndividual.Tasks.Count == config.tasks.Count)
            {
                int t = 0;
                bool different = false;
                foreach (Task task in m_bestIndividual.Tasks)
                {
                    if (task.Id != (config.tasks[t]).Id)
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
            m_initialMutationRate = (int) (2.5*computeOptimalPopulationSize(m_tasks.Count));
            m_mutationRate = m_initialMutationRate;
            m_initialCataclysmCountdown = m_initialMutationRate*10;
            m_cataclysmCountdown = m_initialCataclysmCountdown;
        }

        private void configureTaskUserIds()
        {
            for (int t = 0; t < m_tasks.Count; t++)
            {
                m_tasks[t].UserId = t;
            }
        }

        /// <summary>
        /// Regenerate new sequences but keep the current best
        /// </summary>
        private void regeneratePopulation(bool keepBest)
        {
            int bestFitness = Int32.MaxValue;

            if (m_bestIndividual != null)
            {
                bestFitness = m_bestIndividual.Fitness;
            }

            if (!keepBest)
            {
                m_bestIndividual = null;
                m_bestFitness = Int32.MaxValue;
            }

            int firstIndex = 0;
            if (keepBest && m_bestIndividual != null && m_bestIndividual.Id >= m_populationSize)
            {
                firstIndex = 1;
                m_individuals[0] = m_bestIndividual;
                m_bestIndividual.Id = 0;
            }


            for (int t = firstIndex; t < m_populationSize; t++)
            {
                if (m_individuals[t] == null)
                {
                    m_individuals[t] = generateInitialSequenceUsingCheapestInsert();
                }
                else
                {
                    if (keepBest && m_bestIndividual != null && m_individuals[t].Id == m_bestIndividual.Id)
                    {
                    }
                    else
                    {
                        m_individuals[t].Robot = m_robot;
                        m_individuals[t].StartX = m_startX;
                        m_individuals[t].StartY = m_startY;
                        m_nearestInsert.generate(new List<Task>(m_tasks), m_tasks.Count, m_individuals[t]);
                    }
                }
                m_individuals[t].Id = t;
            }

            computeMinSequence();

            if (keepBest && m_bestFitness > bestFitness)
            {
                int a = 0;
            }
        }


        protected override void regeneratePopulation()
        {
            regeneratePopulation(true);
        }


        protected override int computeMaxChildren()
        {
            return (int) (m_populationSize*0.05 + 2);
        }

        protected override int computeMaxMutations()
        {
            return 0;
        }

        private void computeMinSequence()
        {
            int bestFitness = Int32.MaxValue;
            TaskSequence bestIndividual = null;

            for (int t = 0; t < m_populationSize; t++)
            {
                if (m_individuals[t] != null && m_individuals[t].Fitness < bestFitness)
                {
                    bestFitness = m_individuals[t].Fitness;
                    bestIndividual = m_individuals[t];
                }
            }

            onNewBestIndividual(bestIndividual);
        }

        public void recomputeFitness()
        {
            int bestFitness = Int32.MaxValue;
            TaskSequence bestIndividual = null;

            for (int t = 0; t < m_populationSize; t++)
            {
                if (m_individuals[t] != null)
                {
                    m_individuals[t].updateFitness();

                    if (m_individuals[t].Fitness < bestFitness)
                    {
                        bestFitness = m_individuals[t].Fitness;
                        bestIndividual = m_individuals[t];
                    }
                }
            }

            onNewBestIndividual(bestIndividual);
        }

        public List<Task> getOriginalTasks()
        {
            return m_tasks;
        }

        private TaskSequence generateInitialSequenceUsingCheapestInsert()
        {
            TaskSequence sequence = m_nearestInsert.generate(new List<Task>(m_tasks), m_tasks.Count, m_robot, m_startX,
                                                             m_startY, m_fitnessLevels);
            sequence.TaskSequencer = this;
            return sequence;
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public int expectedFitness;
            public FitnessLevels fitnessLevels;
            public bool orderedTasks = false;
            public Robot robot;
            public double startX;
            public double startY;
            public List<Task> tasks;
        }

        #endregion
    }
}