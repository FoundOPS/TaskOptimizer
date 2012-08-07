using System;
using System.Collections;
using System.Text;
using System.Timers;

namespace TaskOptimizer.Model
{
    class TaskSequencer
    {
        public class Configuration
        {
            public Robot robot;
            public ArrayList tasks;
            public int startX;
            public int startY;
        }

        public TaskSequencer(int maxTasks)
        {
            m_crossoverAddedTasks = new bool[maxTasks];
            m_crossoverTaskNeighborMatrix = new bool[maxTasks][];
            m_crossoverTaskNeighbors = new Task[maxTasks][];
            m_crossoverTaskNeighborCount = new int[maxTasks];
            for (int t = 0; t < maxTasks; t++)
            {                
                m_crossoverTaskNeighborMatrix[t] = new bool[maxTasks];
                m_crossoverTaskNeighbors[t] = new Task[4];                
            }
            
            m_sequences = new TaskSequence[m_maxPopulationSize];
        }       

        public void init(TaskSequencer.Configuration config)
        {
            if (!isConfigurationChanged(config))
            {
                return;
            }

            m_robot = config.robot;
            m_tasks = config.tasks.Clone() as ArrayList;
            m_startX = config.startX;
            m_startY = config.startY;

            configureMutationParameters();
            configureTaskUserIds();
            regeneratePopulation();                 
        }

        public int PopulationSize
        {
            get
            {
                return m_populationSize;
            }

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
                regeneratePopulation();
            }
        }

       

        public int MaxPopulationSize
        {
            get
            {
                return m_maxPopulationSize;
            }
        }

        public TaskSequence MinTaskSequence
        {
            get
            {
                return m_minSequence;
            }
        }

        public void recomputeCost()
        {
            m_minCost = Int32.MaxValue;

            foreach (TaskSequence sequence in m_sequences)
            {
                if (sequence != null)
                {
                    sequence.updateCost();

                    if (sequence.Cost < m_minCost)
                    {
                        m_minSequence = sequence;
                        m_minCost = sequence.Cost;
                    }
                }
            }            
        }

        public TaskSequence compute()
        {

            
            if (m_tasks.Count == 0)
            {
                return null;
            }

            TaskSequence parent;
            int nbChilds = 0;
            int maxChilds = (int)(m_populationSize * 0.05 + 2);
            while (nbChilds < maxChilds && (parent = selectParents()) != null)
            {
                TaskSequence child = selectDead();
                
                if (child.Id != parent.Id && child.Id != m_minSequence.Id)
                {

                    //TaskSequence child = generateInitialSequence();//crossover(m_sequences[p1] as TaskSequence, m_sequences[p2] as TaskSequence);
                    //TaskSequence child = crossover(m_sequences[p1] as TaskSequence, m_sequences[p2] as TaskSequence);
                    
                    crossover(m_minSequence, parent, child);
                    if (child.Cost < m_minCost)
                    {
                        m_minCost = child.Cost;
                        m_minSequence = child;
                        m_mutationRate = m_initialMutationRate;
                        m_cataclysmCountdown = m_initialCataclysmCountdown;
                    }
                    nbChilds++;

                    if (m_rand.Next(m_mutationRate) == 0)
                    {
                        mutate2(selectDead());
                    }
                   
                }
                
            }

            m_mutationRate--;
            m_cataclysmCountdown--;

            if (m_cataclysmCountdown == 0)
            {
                for (int t = 0; t < m_populationSize; t++)
                {
                    
                    if (m_sequences[t].Id != m_minSequence.Id)
                    {
                        m_sequences[t] = generateInitialSequenceUsingCheapestInsert();
                        m_sequences[t].Id = t;
                        if (m_sequences[t].Cost < m_minCost)
                        {
                            m_minCost = m_sequences[t].Cost;
                            m_minSequence = m_sequences[t];
                            m_mutationRate = m_initialMutationRate;
                        }
                    }
                }

                m_cataclysmCountdown = m_initialCataclysmCountdown;
            }

            if (m_mutationRate < 1)
            {
                m_mutationRate = 1;
            }

            return m_minSequence;
            
        }

        private bool isConfigurationChanged(TaskSequencer.Configuration config)
        {
            if (m_minSequence != null && m_minSequence.Tasks.Count == config.tasks.Count)
            {
                int t = 0;
                bool different = false;
                foreach (Task task in m_minSequence.Tasks)
                {
                    if (task.Id != (config.tasks[t] as Task).Id)
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
            m_mutationRate = m_initialMutationRate;
            m_initialCataclysmCountdown = m_initialMutationRate * 5;
            m_cataclysmCountdown = m_initialCataclysmCountdown;
        }

        private void configureTaskUserIds()
        {
            for (int t = 0; t < m_tasks.Count; t++)
            {
                (m_tasks[t] as Task).UserId = t;
            }
        }       

        /// <summary>
        /// Regenerate new sequences but keep the current best
        /// </summary>
        private void regeneratePopulation()
        {
            int firstIndex = 0;

            if( m_minSequence != null )
            {
                m_sequences[0] = m_minSequence;
                m_sequences[0].Id = 0;
                firstIndex = 1;
            }

            for (int t = firstIndex; t < m_populationSize; t++)
            {
                m_sequences[t] = generateInitialSequenceUsingCheapestInsert();
                m_sequences[t].Id = t;
            }

            // clear unused sequences...
            for (int t = m_populationSize; t < m_maxPopulationSize; t++)
            {
                m_sequences[t] = null;
            }

            computeMinSequence();
        }

        private void mutate(TaskSequence sequence)
        {
            int nbMutations = m_rand.Next(sequence.Tasks.Count / 5)+1;
            for(int t=0; t<nbMutations; t++)
            {
                int t1 = m_rand.Next(sequence.Tasks.Count);
                int t2 = m_rand.Next(sequence.Tasks.Count);

                Object temp = sequence.Tasks[t1];
                sequence.Tasks[t1] = sequence.Tasks[t2];
                sequence.Tasks[t2] = temp;
            }
        }

        private void mutate2(TaskSequence sequence)
        {
            
            int nbMutations = m_rand.Next(sequence.Tasks.Count / 5) + 1;
            for (int t = 0; t < nbMutations; t++)
            {
                int t1 = m_rand.Next(sequence.Tasks.Count);
                int range = (int)(m_rand.Next((int)(sequence.Tasks.Count*0.3))-sequence.Tasks.Count*0.15-1);
                //int range = (int)(m_rand.Next(5) - 2);
                int t2 = (t1 + range) % sequence.Tasks.Count;
                if (t2 < 0)
                {
                    t2 = sequence.Tasks.Count + t2;
                }
                Object temp = sequence.Tasks[t1];
                sequence.Tasks[t1] = sequence.Tasks[t2];
                sequence.Tasks[t2] = temp;
            }
        }

        private void computeMinSequence()
        {
            m_minCost = Int32.MaxValue;
            m_minSequence = null;
            foreach (TaskSequence sequence in m_sequences)
            {
                if (sequence != null && sequence.Cost < m_minCost)
                {
                    m_minSequence = sequence;
                    m_minCost = sequence.Cost;
                }
            }
        }



        private TaskSequence selectParents()
        {            
            int curIndex = m_rand.Next(m_populationSize);
            for (int t = 0; t < m_populationSize; t++)
            {               
                TaskSequence sequence = m_sequences[curIndex % m_populationSize];
                if (sequence.Id != m_minSequence.Id)
                {
                    int odds = (sequence.Cost * 3 / (m_minCost)) + 1;

                    int n = m_rand.Next(odds);

                    if (n == 0)
                    {
                        return sequence;
                    }
                }

                curIndex++;
            }

            return null;
        }

        private TaskSequence selectDead()
        {
            int curIndex = m_rand.Next(m_populationSize);
            while( true )
            {
                TaskSequence sequence = m_sequences[curIndex % m_populationSize];
                if (sequence.Id != m_minSequence.Id)
                {
                    int odds = ((m_minCost * 5) / (sequence.Cost)) + 1;

                    int n = m_rand.Next(odds);

                    if (n == 0)
                    {
                        return sequence;
                    }
                }

                curIndex++;
            }

            return null;
        }

        private TaskSequence generateInitialSequenceUsingCheapestInsert()
        {
            return m_nearestInsert.generate(m_tasks.Clone() as ArrayList, m_tasks.Count, m_robot, m_startX, m_startY);
        }

        
       

        private void crossover(TaskSequence p1, TaskSequence p2, TaskSequence child)
        {
            // use the Edge recombination operator 
            // see: http://en.wikipedia.org/wiki/Edge_recombination_operator
            //m_rand = new Random(11);
            for (int t = 0; t < m_tasks.Count; t++)
            {
                for (int t2 = 0; t2 < m_tasks.Count; t2++)
                {
                    m_crossoverTaskNeighborMatrix[t][t2] = false;
                }
                m_crossoverAddedTasks[t] = false;
                m_crossoverTaskNeighborCount[t] = 0;

            }

            child.Tasks.Clear();

            addNeighborsToCrossoverList(p1);
            addNeighborsToCrossoverList(p2);
            Task task = null;

            

            if (m_rand.Next(2) == 0)
            {
                task = p1.Tasks[0] as Task;
            }
            else
            {
                task = p2.Tasks[0] as Task;
            }            

            for (int i=0; i<m_tasks.Count; i++)
            {
                child.Tasks.Add(task);
                m_crossoverAddedTasks[task.UserId] = true;

                int nbNeighbors = m_crossoverTaskNeighborCount[task.UserId];

                if (nbNeighbors > 0)
                {
                    // find the neighbor with the fewest neighbors...

                    Task minTask = null;
                    int minTaskNeighbors = 99999;

                    // remove the task from its neighbors...
                    for (int t = 0; t < 4; t++)
                    {
                        Task neighbor = m_crossoverTaskNeighbors[task.UserId][t];

                        if (neighbor != null  && m_crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId])
                        {
                            m_crossoverTaskNeighborMatrix[neighbor.UserId][task.UserId] = false;
                            m_crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId] = false;
                            m_crossoverTaskNeighborCount[neighbor.UserId]--;

                            if  (m_crossoverTaskNeighborCount[neighbor.UserId] < minTaskNeighbors ||
                                (m_crossoverTaskNeighborCount[neighbor.UserId] == minTaskNeighbors && m_rand.Next(2) == 0))
                            {
                                minTask = neighbor;
                                minTaskNeighbors = m_crossoverTaskNeighborCount[neighbor.UserId];
                            }
                        }
                    }                   

                    task = minTask;
                }
                else
                {
                    // find a task not already added...

                    for (int notAddedTaskIndex = 0; notAddedTaskIndex < m_tasks.Count; notAddedTaskIndex++)
                    {
                        if (m_crossoverAddedTasks[notAddedTaskIndex] == false)
                        {
                            task = m_tasks[notAddedTaskIndex] as Task;
                            break;
                        }
                    }
                }
            }

            child.updateCost();

           

        }

        private void addNeighborsToCrossoverList(TaskSequence sequence)
        {
            Task prevTask = null;
            foreach (Task task in sequence.Tasks)
            {
                if (prevTask != null)
                {
                    if (m_crossoverTaskNeighborMatrix[task.UserId][prevTask.UserId] == false)
                    {
                        m_crossoverTaskNeighborMatrix[task.UserId][prevTask.UserId] = true;
                        m_crossoverTaskNeighbors[task.UserId][m_crossoverTaskNeighborCount[task.UserId]] = prevTask;
                        m_crossoverTaskNeighborCount[task.UserId]++;
                    }

                    if (m_crossoverTaskNeighborMatrix[prevTask.UserId][task.UserId] == false)
                    {
                        m_crossoverTaskNeighborMatrix[prevTask.UserId][task.UserId] = true;
                        m_crossoverTaskNeighbors[prevTask.UserId][m_crossoverTaskNeighborCount[prevTask.UserId]] = task;
                        m_crossoverTaskNeighborCount[prevTask.UserId]++;
                    }
                   
                }

                prevTask = task;
            }
        }
        private int m_mutationRate;
        private int m_initialCataclysmCountdown = 1000;
        private int m_initialMutationRate = 50;
        private int m_cataclysmCountdown;
        private int m_populationSize = 1;

        private int m_maxPopulationSize = 25;
        private Task[][] m_crossoverTaskNeighbors;
        private int[] m_crossoverTaskNeighborCount;
        private bool[][] m_crossoverTaskNeighborMatrix;
        private bool[] m_crossoverAddedTasks; 
        private Robot m_robot;
        private Random m_rand = new Random();
        private ArrayList m_tasks;
        private int m_startX, m_startY;
        private TaskSequence[] m_sequences;
        private int m_minCost = Int32.MinValue;
        private TaskSequence m_minSequence = null;

        private TaskSequencerNearestInsert m_nearestInsert = new TaskSequencerNearestInsert();      

    }
}
