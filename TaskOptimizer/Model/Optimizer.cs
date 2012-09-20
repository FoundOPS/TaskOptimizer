using System;
using System.Collections.Generic;
using System.Threading;

namespace TaskOptimizer.Model
{
    public class Optimizer
    {
        public Thread m_creationThread;
        private int m_curIteration;
        private TaskDistributor[] m_distributors;
        private bool m_endOptimizeThread;
        public TaskDistributor m_minDistributor;
        private int m_minDistributorFitness = Int32.MaxValue;
        private Random m_rand;
        private bool[] m_recomputeFitnesses;
        private Thread[] m_threads;

        public Optimizer(Configuration config)
        {
            create(config);
        }

        public int TotalTime
        {
            get { return m_minDistributor.TotalTime; }
        }

        public int TotalCost
        {
            get { return m_minDistributor.TotalCost; }
        }

        public int CurrentIteration
        {
            get { return m_curIteration; }
        }

        public List<TaskSequence> MinSequences
        {
            get { return new List<TaskSequence>(m_minDistributor.MinSequences); }
        }

        public int Fitness
        {
            get { return m_minDistributor.Fitness; }
        }

        public int NbUsedRobots
        {
            get { return m_minDistributor.NbUsedRobots; }
        }

        public Boolean stillInit()
        {
            if (m_creationThread.ThreadState != ThreadState.Stopped) return true;
            return false;
        }

        private void create(Configuration config)
        {
            m_rand = new Random(config.randomSeed);

            m_distributors = new TaskDistributor[config.nbDistributors];

            m_threads = new Thread[config.nbDistributors];
            m_recomputeFitnesses = new bool[config.nbDistributors];

            var taskDistributorConfiguration = new TaskDistributor.Configuration();
            taskDistributorConfiguration.robots = config.robots;
            taskDistributorConfiguration.tasks = config.tasks;
            taskDistributorConfiguration.startX = config.startX;
            taskDistributorConfiguration.startY = config.startY;
            taskDistributorConfiguration.fitnessLevels = config.fitnessLevels;
            taskDistributorConfiguration.optimizer = this;

            for (int t = 0; t < config.nbDistributors; t++)
            {
                Console.WriteLine(t);


                taskDistributorConfiguration.randomSeed = m_rand.Next();
                taskDistributorConfiguration.startProgressPercent = t*100/config.nbDistributors;
                taskDistributorConfiguration.endProgressPercent = taskDistributorConfiguration.startProgressPercent +
                                                                  100/config.nbDistributors;
                m_distributors[t] = new TaskDistributor(taskDistributorConfiguration);

                m_recomputeFitnesses[t] = false;
                m_threads[t] = new Thread(optimizeThread);
            }


            start();
        }

        public void recomputeFitness()
        {
            for (int t = 0; t < m_recomputeFitnesses.Length; t++)
            {
                m_recomputeFitnesses[t] = true;
            }
        }

        private void start()
        {
            compute();
            m_endOptimizeThread = false;
            for (int t = 0; t < m_threads.Length; t++)
            {
                m_threads[t].Start(t);
            }
        }

        public void compute()
        {
            int nbIterations = 0;
            for (int t = 0; t < m_distributors.Length; t++)
            {
                nbIterations += m_distributors[t].CurrentIteration;
                if (m_distributors[t].Fitness < m_minDistributorFitness)
                {
                    m_minDistributorFitness = m_distributors[t].Fitness;
                    m_minDistributor = m_distributors[t];
                }
            }

            m_curIteration = nbIterations;
        }

        public void stop()
        {
            m_endOptimizeThread = true;
            for (int t = 0; t < m_threads.Length; t++)
            {
                if (m_threads[t] != null && m_threads[t].IsAlive)
                {
                    if (m_threads[t] != null)
                    {
                        m_threads[t].Join();
                    }
                }
            }
        }

        private void optimizeThread(object index)
        {
            var distributorIndex = (int) index;

            TaskDistributor distributor = m_distributors[distributorIndex];

            while (!m_endOptimizeThread)
            {
                if (m_recomputeFitnesses[distributorIndex])
                {
                    distributor.recomputeFitness();
                    m_recomputeFitnesses[distributorIndex] = false;
                }
                distributor.optimize();
            }
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public FitnessLevels fitnessLevels;
            public int nbDistributors;
            public int randomSeed;
            public List<Robot> robots;
            public double startX, startY;
            public List<Task> tasks;
        }

        #endregion
    }
}