
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskDistribution : Individual
    {
        public class Configuration
        {
            public List<Robot> robots;
            public List<Task> tasks;
            public double startX;
            public double startY;
            public FitnessLevels fitnessLevels;
            public int randomSeed;
            public TaskDistributor distributor;
        }

        public TaskDistribution(TaskDistribution.Configuration config)
        {
            configure(config);
        }

        public TaskDistribution(TaskDistribution distribution)
        {
            Configuration config = new Configuration();

            config.robots = distribution.m_robots;
            Console.WriteLine(config.robots.Count);
            config.tasks = distribution.m_tasks;
            config.startX = distribution.m_startX;
            config.startY = distribution.m_startY;
            config.fitnessLevels = distribution.m_fitnessLevels;
            config.randomSeed = distribution.m_rand.Next();
            config.distributor = distribution.m_distributor;

            Id = distribution.Id;

            configure(config);
            Console.WriteLine("Configured");
            m_fitness = Int32.MaxValue;
            Console.WriteLine(distribution.Sequences.Count);
            setSequences(distribution.Sequences);

        }

        public void setSequences(List<TaskSequence> sequences)
        {
            int fitness = m_fitness;
            int robotIndex = 0;
            Console.WriteLine(sequences.Count);
            foreach (TaskSequence sequence in sequences)
            {
                if (sequence != null)
                {
                    
                    foreach (Task task in sequence.Tasks)
                    {
                        m_taskDistributedRobot[task.Id] = robotIndex;
                    }
                }
                robotIndex++;
            }

            updateDistributedTasksFromDistribution();
            
            for (int t = 0; t < m_robots.Count; t++)
            {
                TaskSequencer.Configuration config = new TaskSequencer.Configuration();
                config.robot = m_robots[t];
                if (m_distributedTasks[t].Count > 0 & sequences[t]!=null)
                {
                    config.tasks = cloneTaskList(sequences[t].Tasks);
                   
                    config.expectedFitness = (sequences[t]).Fitness;
                    config.orderedTasks = true;
                }
                else
                {
                    config.tasks = new List<Task>();
                }
                config.startX = m_startX;
                config.startY = m_startY;
                config.fitnessLevels = m_fitnessLevels;


                m_sequencers[t].configure(config);

            }
           
            optimize();
        }

        public int Id
        {
            get
            {
                return m_id;
            }

            set
            {
                m_id = value;
            }
        }




        public void generateInitialSolution()
        {
            int method = m_rand.Next(10);

            if (method < 1)
            {
                generateFixedInitialSolution(m_rand.Next(m_robots.Count));
            }
            else if (method < 5)
            {
                generateRandomInitialSolution();
            }
            else
            {
                generateKMeanInitialSolution();
            }
        }



        /// <summary>
        /// All tasks for one robot.
        /// </summary>        
        public void generateFixedInitialSolution(int robotIndex)
        {
            for (int t = 0; t < m_tasks.Count; t++)
            {
                m_taskDistributedRobot[t] = robotIndex;
            }

            afterDistributionChanged();
        }



        public bool OptimizeSequences
        {
            set
            {

                foreach (TaskSequencer sequencer in m_sequencers)
                {
                    if (value)
                    {
                        m_sequencers[0].useOptimalPopulationSize();
                    }
                    else
                    {
                        m_sequencers[0].PopulationSize = 1;
                    }
                }

                m_optimizingSequences = value;
            }

            get
            {
                return m_optimizingSequences;
            }
        }

        public int NbUsedRobots
        {
            get
            {
                return m_nbUsedRobots;
            }
        }


        public int Fitness
        {
            get
            {
                return m_fitness;
            }
        }

        public int TotalTime
        {
            get
            {
                return m_totalTime;
            }
        }

        public int TotalCost
        {
            get
            {
                return m_totalCost;
            }
        }

        public List<TaskSequence> Sequences
        {
            get
            {
                return m_sequences;
            }
        }



        public void recomputeFitness()
        {
            foreach (TaskSequencer sequencer in m_sequencers)
            {
                sequencer.recomputeFitness();
            }

            optimize();
        }

        public void optimize()
        {
            m_fitness = 0;
            int maxTime = 0;
            int usedRobots = 0;

            for (int t = 0; t < m_robots.Count; t++)
            {
                m_sequencers[t].optimize();

                TaskSequence sequence = m_sequencers[t].MinTaskSequence;
                if (sequence != null)
                {
                    m_fitness += sequence.Cost;

                    if (sequence.Time > maxTime)
                    {
                        maxTime = sequence.Time;
                    }

                    if (sequence.Tasks.Count > 0)
                    {
                        usedRobots++;
                    }
                }

                m_sequences[t] = sequence;
            }

            m_nbUsedRobots = usedRobots;
            m_totalCost = m_fitness;

            m_fitness *= m_fitnessLevels.CostMultiplier;
            m_fitness += maxTime * m_fitnessLevels.TimeMultiplier;


            m_totalTime = maxTime;
        }

        /// <summary>
        /// We become the child of these 2 parent distributions
        /// </summary>        
        public void crossover(Individual parent1, Individual parent2)
        {
            TaskDistribution p1 = parent1 as TaskDistribution;
            TaskDistribution p2 = parent2 as TaskDistribution;

            bool changed = false;

            for (int t = 0; t < m_tasks.Count; t++)
            {
                int distributedRobotBefore = m_taskDistributedRobot[t];

                if (m_rand.Next() % 2 == 0)
                {
                    m_taskDistributedRobot[t] = p1.m_taskDistributedRobot[t];
                }
                else
                {
                    m_taskDistributedRobot[t] = p2.m_taskDistributedRobot[t];
                }

                if (distributedRobotBefore != m_taskDistributedRobot[t])
                {
                    changed = true;
                }
            }

            if (changed)
            {
                afterDistributionChanged();
            }
            else
            {
                // population surely is very similar... introduce some variations!
                mutate();
            }


        }

        public Task findNearestAssignedTask(Task fromTask)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (Task task in m_tasks)
            {
                // assigned?
                if (m_taskDistributedRobot[task.Id] != -1 && fromTask.Id != task.Id)
                {
                    int distance = task.distanceTo(fromTask);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minTask = task;
                    }
                }
            }



            return minTask;

        }


        public void mutate()
        {

            int force = m_distributor.computeMutationForce();

            if (force > 10)
            {
                force = 10;
            }

            int nbMutations = (int)(m_rand.Next(m_tasks.Count / 10) * (force / 3.0) + 1);
            bool changed = false;
            double nearestTolerance = 1.0 + force / 12.0;
            for (int t = 0; t < nbMutations; t++)
            {
                int taskIndex = m_rand.Next(m_tasks.Count);

                Task nearestTask = m_nearestInsert.findAlmostNearestTask(m_tasks[taskIndex], m_tasks, nearestTolerance);
                if (m_taskDistributedRobot[taskIndex] != m_taskDistributedRobot[nearestTask.Id])
                {
                    // switch the task to a close robot
                    m_taskDistributedRobot[taskIndex] = m_taskDistributedRobot[nearestTask.Id];
                    changed = true;
                }
                else
                {
                    // assign a random robot...
                    m_taskDistributedRobot[taskIndex] = m_rand.Next(m_robots.Count);
                    changed = true;
                }
            }

            if (changed)
            {
                afterDistributionChanged();
            }
        }

        private void configure(TaskDistribution.Configuration config)
        {
            m_rand = new Random(config.randomSeed);

            configureTasks(config);

            m_fitnessLevels = config.fitnessLevels;
            m_robots = config.robots;
            m_startX = config.startX;
            m_startY = config.startY;
            m_distributor = config.distributor;

            m_taskDistributedRobot = new int[m_tasks.Count];
            m_lastTaskDistributedRobot = new int[m_tasks.Count];
            m_sequencers = new TaskSequencer[m_robots.Count];
            m_distributedTasks = new List<Task>[m_robots.Count];
            m_sequences = new List<TaskSequence>(m_robots.Count);

            for (int t = 0; t < m_robots.Count; t++)
            {
                m_distributedTasks[t] = new List<Task>(m_tasks.Count);
                m_sequencers[t] = new TaskSequencer(m_tasks.Count);
                m_sequences.Add(null);
            }

            for (int t = 0; t < m_tasks.Count; t++)
            {
                m_lastTaskDistributedRobot[t] = -1;
            }
        }

        private void configureTasks(TaskDistribution.Configuration config)
        {
            m_tasks = cloneTaskList(config.tasks);
        }

        private List<Task> cloneTaskList(List<Task> tasks)
        {
            List<Task> clonedTasks = new List<Task>(tasks.Count);
            foreach (Task task in tasks)
            {
                clonedTasks.Add(new Task(task));
            }

            return clonedTasks;
        }


        private void generateKMeanInitialSolution()
        {
            if (m_robots.Count < 2)
            {
                // no need to cluster!
                generateFixedInitialSolution(0);
                return;
            }

            KMeanCluster clusterEngine = new KMeanCluster();
           // int nbRobots = m_rand.Next(m_robots.Count * 2);
           // if (nbRobots < 2)
            //{
            //    nbRobots = 2;
           // }

           // if (nbRobots > m_robots.Count)
            //{
               int nbRobots = m_robots.Count;
            //}

            clusterEngine.compute(m_tasks, nbRobots, m_taskDistributedRobot);
            afterDistributionChanged();
        }

        private void generateRandomInitialSolution()
        {
            if (m_robots.Count < 2)
            {
                // no need for random!
                generateFixedInitialSolution(0);
                return;
            }

            List<Task> remainingTasks = new List<Task>(m_tasks);

            int robotIndex = m_rand.Next(m_robots.Count);

            int divisor = m_robots.Count / 3 - 1;


            if (divisor > m_tasks.Count / 1)
            {
                divisor = m_tasks.Count / 1;
            }

            if (divisor < 2)
            {
                divisor = 2;
            }

            int maxTasksPerRobot = (int)(m_tasks.Count / (double)m_robots.Count);

            for (int t = 0; t < m_robots.Count; t++)
            {

                int nbTasks = maxTasksPerRobot;


                if (nbTasks > remainingTasks.Count)
                {
                    nbTasks = remainingTasks.Count;
                }

                if (t == m_robots.Count - 1)
                {
                    nbTasks = remainingTasks.Count;
                }

                if (nbTasks > 0)
                {
                    TaskSequence sequence = m_nearestInsert.generate(remainingTasks, nbTasks, m_robots[robotIndex], m_startX, m_startY, m_fitnessLevels);

                    foreach (Task task in sequence.Tasks)
                    {
                        m_taskDistributedRobot[task.Id] = robotIndex;
                    }
                }

                robotIndex = (robotIndex + 1) % m_robots.Count;
            }

            afterDistributionChanged();

        }

        private bool isLastDistributionEqualsNewOne()
        {
            bool changed = false;
            for (int t = 0; t < m_lastTaskDistributedRobot.Length; t++)
            {
                if (m_lastTaskDistributedRobot[t] != m_taskDistributedRobot[t])
                {
                    changed = true;
                    break;
                }
            }

            return !changed;
        }

        private void afterDistributionChanged()
        {
            if (isLastDistributionEqualsNewOne())
            {
                return;
            }

            updateDistributedTasksFromDistribution();

            configureSequencersFromDistribution();

        }

        private void updateDistributedTasksFromDistribution()
        {
            clearDistributedTasks();

            for (int t = 0; t < m_tasks.Count; t++)
            {
                m_distributedTasks[m_taskDistributedRobot[t]].Add(m_tasks[t]);
               
                m_lastTaskDistributedRobot[t] = m_taskDistributedRobot[t];
                
            }
            
        }

        private void clearDistributedTasks()
        {
            for (int t = 0; t < m_robots.Count; t++)
            {
                m_distributedTasks[t].Clear();
            }
        }

        private void configureSequencersFromDistribution()
        {
            for (int t = 0; t < m_robots.Count; t++)
            {
                TaskSequencer.Configuration config = new TaskSequencer.Configuration();
                config.robot = m_robots[t];
                config.tasks = m_distributedTasks[t];
                config.startX = m_startX;
                config.startY = m_startY;
                config.fitnessLevels = m_fitnessLevels;
                m_sequencers[t].configure(config);
            }

            optimize();
        }

        public int[] m_lastTaskDistributedRobot;

        public int m_nbUsedRobots = 0;
        public int m_id;
        public FitnessLevels m_fitnessLevels;
        public int m_totalTime, m_totalCost;
        public int[] m_taskDistributedRobot;
        public List<Task>[] m_distributedTasks;
        public List<Robot> m_robots;
        public List<TaskSequence> m_sequences;
        public List<Task> m_tasks;
        public TaskSequencer[] m_sequencers;
        public Random m_rand;
        public double m_startX, m_startY;
        public int m_fitness = 0;
        public bool m_optimizingSequences = false;
        public TaskDistributor m_distributor;
        TaskSequencerNearestInsert m_nearestInsert = new TaskSequencerNearestInsert();
    }
}