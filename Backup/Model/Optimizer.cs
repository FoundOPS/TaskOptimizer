using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class Optimizer
    {
        public class Configuration
        {
            public List<Robot> robots;
            public List<Task> tasks;
            public int startX, startY;
            public FitnessLevels fitnessLevels;
            public int nbDistributors;
            public int randomSeed;
            public WorkProgress progress;
        }

        public Optimizer(Configuration config)
        {
            ThreadStart start = delegate { create(config); };
            m_creationThread = new Thread(start);
            m_creationThread.Start();            
        }

        Thread m_creationThread;

        private void create(Configuration config)
        {
            config.progress.onWorkProgress("Creating arrays...", 0);
            m_rand = new Random(config.randomSeed);

            m_distributors = new TaskDistributor[config.nbDistributors];
            m_threads = new Thread[config.nbDistributors];
            m_recomputeFitnesses = new bool[config.nbDistributors];

            TaskDistributor.Configuration taskDistributorConfiguration = new TaskDistributor.Configuration();
            taskDistributorConfiguration.robots = config.robots;
            taskDistributorConfiguration.tasks = config.tasks;
            taskDistributorConfiguration.startX = config.startX;
            taskDistributorConfiguration.startY = config.startY;
            taskDistributorConfiguration.fitnessLevels = config.fitnessLevels;
            taskDistributorConfiguration.optimizer = this;
            taskDistributorConfiguration.progress = config.progress;
            
            for (int t = 0; t < config.nbDistributors; t++)
            {
                if (config.progress.WorkCancelled)
                {                    
                    config.progress.onWorkEnd();
                    return;
                }

                config.progress.onWorkProgress("Generating distributors...", t * 100 / config.nbDistributors);

                taskDistributorConfiguration.randomSeed = m_rand.Next();
                taskDistributorConfiguration.startProgressPercent = t * 100 / config.nbDistributors;
                taskDistributorConfiguration.endProgressPercent = taskDistributorConfiguration.startProgressPercent + 100 / config.nbDistributors;
                m_distributors[t] = new TaskDistributor(taskDistributorConfiguration);
                m_recomputeFitnesses[t] = false;
                m_threads[t] = new Thread(optimizeThread);
            }

            config.progress.onWorkProgress("Starting...", 100);
            
            start();
            
            config.progress.onWorkEnd();

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
                m_threads[t].Start((object)t);
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
            int distributorIndex = (int)index;

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
         


        public int TotalTime
        {
            get
            {
                return m_minDistributor.TotalTime;
            }
        }

        public int TotalCost
        {
            get
            {
                return m_minDistributor.TotalCost;
            }
        }

        public int CurrentIteration
        {
            get
            {                
                return m_curIteration;
            }
        }

        public List<TaskSequence> MinSequences
        {
            get
            {
                return new List<TaskSequence>(m_minDistributor.MinSequences);
            }
        }

        public int Fitness
        {
            get
            {
                return m_minDistributor.Fitness;
            }
        }

        public int NbUsedRobots
        {
            get
            {
                return m_minDistributor.NbUsedRobots;
            }
        }

        private Thread[] m_threads;
        private bool m_endOptimizeThread = false;
        private Random m_rand;
        
        private TaskDistributor[] m_distributors;
        private bool[] m_recomputeFitnesses;
        private TaskDistributor m_minDistributor;
        private int m_minDistributorFitness = Int32.MaxValue;
        private int m_curIteration = 0;
    }
}
