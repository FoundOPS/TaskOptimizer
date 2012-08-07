using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskDistributor : Population<TaskDistribution>
    {
        public class Configuration
        {
            public List<Robot> robots;
            public List<Task> tasks;
            public int startX;
            public int startY;
            public FitnessLevels fitnessLevels;
            public Optimizer optimizer;
            public int randomSeed;
            public WorkProgress progress;
            public int startProgressPercent, endProgressPercent;
        }

        public TaskDistributor(TaskDistributor.Configuration config)
        {
            configure(config);          
        }       

        public void recomputeFitness()
        {
            int bestFitness = Int32.MaxValue;
            TaskDistribution bestDistribution = null;
            foreach (TaskDistribution distribution in m_individuals)
            {
                distribution.recomputeFitness();
                if (distribution.Fitness < bestFitness)
                {
                    bestDistribution = distribution;
                    bestFitness = distribution.Fitness;
                }
            }

            onNewBestIndividual(bestDistribution);
            
        }
               
        public override void optimize()
        {
            computeOptimizationMode();

            DateTime start = DateTime.Now;

            base.optimize();

            if (m_isOptimizingSequences)
            {
                // make sure to update the best copy if has changed...
                onNewBestIndividual(m_bestIndividual);
            }
            
            DateTime end = DateTime.Now;
            TimeSpan elapsed = end - start;           
            

            if (elapsed.TotalMilliseconds > 300)
            {
                int a = 0;
            }
        }

       

        


/******************************************************
 * Attributes */

        public int TotalTime
        {
            get
            {
                return m_bestIndividualCopy.TotalTime;
            }
        }

        public int TotalCost
        {
            get
            {
                return m_bestIndividualCopy.TotalCost;
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
                return m_bestIndividualCopy.Sequences;
            }
        }

        public int Fitness
        {
            get
            {
                return m_bestIndividualCopy.Fitness;
            }
        }

        public int NbUsedRobots
        {
            get
            {
                return m_bestIndividualCopy.NbUsedRobots;
            }
        }

/******************************************************
 * Private */

        protected override void onNewBestIndividual(TaskDistribution individual)
        {
            if (m_bestIndividualCopy != null && m_bestIndividualCopy.Fitness == individual.Fitness)
            {
                // no change!
                return;
            }
            
            TaskDistribution copy = new TaskDistribution(individual);
            if (copy.Fitness != individual.Fitness)
            {
                //throw new Exception("Fitness mismatch when cloning task distribution");
            }

            
            m_bestIndividualCopy = copy;
            base.onNewBestIndividual(individual);
        }

        private void configure(TaskDistributor.Configuration config)
        {
            m_rand = new Random(config.randomSeed);

            m_tasks = config.tasks;
            m_robots = config.robots;
            m_startX = config.startX;
            m_startY = config.startY;
            m_optimizer = config.optimizer;
                        
            configureTaskDistributions(config.fitnessLevels, config.progress, config.startProgressPercent, config.endProgressPercent);
        }

       
        private void configureTaskDistributions(FitnessLevels fitnessLevels, WorkProgress progress, int startProgressPercent, int endProgressPercent)
        {
            configurePopulationSize();

            m_individuals = new TaskDistribution[m_populationSize];

            TaskDistribution.Configuration taskDistributionConfiguration = new TaskDistribution.Configuration();
            taskDistributionConfiguration.robots = m_robots;
            taskDistributionConfiguration.tasks = m_tasks;
            taskDistributionConfiguration.startX = m_startX;
            taskDistributionConfiguration.startY = m_startY;
            taskDistributionConfiguration.fitnessLevels = fitnessLevels;
            taskDistributionConfiguration.distributor = this;
                        
            for (int t = 0; t < m_populationSize; t++)
            {
                if (progress.WorkCancelled)
                {
                    return;
                }

                progress.onWorkProgress("", startProgressPercent + t * (endProgressPercent - startProgressPercent) / m_populationSize);

                taskDistributionConfiguration.randomSeed = m_rand.Next();
                m_individuals[t] = new TaskDistribution(taskDistributionConfiguration);
                m_individuals[t].Id = t;

                if (t < m_robots.Count)
                {
                    m_individuals[t].generateFixedInitialSolution(t);
                }
                else
                {
                    m_individuals[t].generateInitialSolution();
                }
            }

            recomputeFitness();
        }

        private void configurePopulationSize()
        {
            int problemComplexity = m_tasks.Count * m_robots.Count;
            m_populationSize = (int)(10 * Math.Log10(problemComplexity)/Math.Log10(Math.E));
            
            m_initialMutationRate = (int)(1.5 * m_populationSize);
            m_mutationRate = m_initialMutationRate;
            m_initialCataclysmCountdown = m_initialMutationRate * 10;
            m_cataclysmCountdown = m_initialCataclysmCountdown;

            m_maxIterationsWithoutImprovements = m_initialCataclysmCountdown * 10;

            if (m_robots.Count == 1)
            {
                // no need to distribute tasks!
                m_populationSize = 1;
                m_maxIterationsWithoutImprovements = -1;
            }

            
        }    
       

        public int computeMutationForce()
        {
            return (int)((m_initialMutationRate - m_mutationRate) / (double)m_initialMutationRate * 10);
        }

       
        
        protected override int computeMaxMutations()
        {
            return 0;            
        }

        protected override void regeneratePopulation()
        {
            TaskDistribution bestIndividual = null;
            int bestFitness = Int32.MaxValue;

            for (int t = 0; t < m_populationSize; t++)
            {
                // keep the best distribution...
                if (m_individuals[t].Id != m_bestIndividual.Id)
                {
                    if (t != 0)
                    {
                        m_individuals[t].generateInitialSolution();
                    }
                    else
                    {
                        // import a solution from another population?
                        if (m_optimizer.Fitness != Fitness && m_optimizer.Fitness < Fitness * 1.1 && m_rand.Next(2) == 0)
                        {
                            m_individuals[t].setSequences(m_optimizer.MinSequences);
                        }
                    }                    
                }

                if (m_individuals[t].Fitness < bestFitness)
                {
                    bestFitness = m_individuals[t].Fitness;
                    bestIndividual = m_individuals[t];
                }

            }
            
            onNewBestIndividual(bestIndividual);                     
        }

        private void computeOptimizationMode()
        {
            if (!m_isOptimizingSequences && m_nbIterationsWithoutImprovements > m_maxIterationsWithoutImprovements)
            {
                // we've found a possible good distribution solution... so now, optimize the sequences
                m_bestIndividual.OptimizeSequences = true;
                m_nbSequenceOptimizationIterations = 0;
            
                m_isOptimizingSequences = true;
                m_nbSequenceOptimizationIterations++;
            }
            else if (m_robots.Count > 1 && m_isOptimizingSequences && m_nbIterationsWithoutImprovements > 2*m_maxIterationsWithoutImprovements)
            {
                // distribution found! restart distribution optimization!
                foreach (TaskDistribution distribution in m_individuals)
                {
                    distribution.OptimizeSequences = false;
                }
                m_nbIterationsWithoutImprovements = 0;
                m_isOptimizingSequences = false;
            }
        }

        private int m_nbSequenceOptimizationIterations = 0;        
        private bool m_isOptimizingSequences = false;
        private TaskDistribution m_bestIndividualCopy = null;
       
        private List<Task> m_tasks;
        private List<Robot> m_robots;
        private int m_startX, m_startY;
        private Optimizer m_optimizer;
    }
}
