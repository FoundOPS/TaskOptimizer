using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskSequence : Individual
    {

        public TaskSequence()
        {

        }

        public FitnessLevels FitnessLevels
        {
            set
            {
                m_fitnessLevels = value;
            }
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

        public int StartX
        {
            get
            {
                return m_startX;
            }

            set
            {
                m_startX = value;
            }
        }

        public int StartY
        {
            get
            {
                return m_startY;
            }

            set
            {
                m_startY = value;
            }
        }

        public Robot Robot
        {
            get
            {
                return m_robot;
            }

            set
            {
                m_robot = value;
            }
        }

        public int Fitness
        {
            get
            {
                return m_fitness;
            }

            set
            {
                m_fitness = value;
            }
        }

        public int Cost
        {
            get
            {
                return m_cost;
            }

            set
            {
                m_cost = value;
            }
        }

        public int Time
        {
            get
            {
                return m_time;
            }

            set
            {
                m_time = value;
            }
        }

        public List<Task> Tasks
        {
            get
            {
                return m_tasks;
            }

            set
            {
                m_tasks = value;
                configureCrossoverData();
                updateFitness();
            }
        }

        public TaskSequencer TaskSequencer
        {
            set
            {
                m_sequencer = value;
            }

            get
            {
                return m_sequencer;
            }
        }

        private void configureCrossoverData()
        {
            int maxTasks = m_tasks.Count;

            if (m_crossoverAddedTasks != null && m_crossoverAddedTasks.Length >= maxTasks)
            {
                // enough space!
                return;
            }

            m_crossoverAddedTasks = new bool[maxTasks];
            m_crossoverTaskNeighborMatrix = new bool[maxTasks][];
            m_crossoverTaskNeighbors = new Task[maxTasks][];
            m_crossoverTaskNeighborCount = new int[maxTasks];
            for (int t = 0; t < maxTasks; t++)
            {
                m_crossoverTaskNeighborMatrix[t] = new bool[maxTasks];
                m_crossoverTaskNeighbors[t] = new Task[4];
            }
        }


        public void updateFitness()
        {
            int distance = 0;
            int time = 0;
            int cost = 0;
            Task fromTask = null;

            foreach (Task task in Tasks)
            {
                distance += task.distanceTo(fromTask);
                fromTask = task;

                cost += task.Effort * task.Effort * Robot.WorkCost;
                time += task.Effort * task.Effort * Robot.WorkTime;
            }

            cost += distance * Robot.DistanceCost;
            Cost = cost;
            Time = time + distance * Robot.DistanceTime;

            Fitness = Cost * m_fitnessLevels.CostMultiplier + Time * m_fitnessLevels.TimeMultiplier;

        }

        public void crossover(Individual parent1, Individual parent2)
        {
            TaskSequence p1 = parent1 as TaskSequence;
            TaskSequence p2 = parent2 as TaskSequence;

            int beforeFitness = Fitness;

            computeEdgeRecombinaisonCrossover(p1, p2);

            if (beforeFitness == Fitness)
            {
                // no change? means that the parents are very close, so introduce some variety!
                mutate();
            }

        }


        private void computeEdgeRecombinaisonCrossover(TaskSequence p1, TaskSequence p2)
        {
            // use the Edge recombination operator 
            // see: http://en.wikipedia.org/wiki/Edge_recombination_operator
            //m_rand = new Random(11);
            for (int t = 0; t < TaskSequencer.getOriginalTasks().Count; t++)
            {
                for (int t2 = 0; t2 < TaskSequencer.getOriginalTasks().Count; t2++)
                {
                    m_crossoverTaskNeighborMatrix[t][t2] = false;
                }
                m_crossoverAddedTasks[t] = false;
                m_crossoverTaskNeighborCount[t] = 0;

                m_crossoverTaskNeighbors[t][0] = null;
                m_crossoverTaskNeighbors[t][1] = null;
                m_crossoverTaskNeighbors[t][2] = null;
                m_crossoverTaskNeighbors[t][3] = null;
            }

            Tasks.Clear();

            addNeighborsToCrossoverList(p1);
            addNeighborsToCrossoverList(p2);
            Task task = null;

            if (m_rand.Next(2) == 0)
            {
                task = p1.Tasks[0];
            }
            else
            {
                task = p2.Tasks[0];
            }

            for (int i = 0; i < TaskSequencer.getOriginalTasks().Count; i++)
            {
                Tasks.Add(task);
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

                        if (neighbor != null && m_crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId])
                        {
                            m_crossoverTaskNeighborMatrix[neighbor.UserId][task.UserId] = false;
                            m_crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId] = false;
                            m_crossoverTaskNeighborCount[neighbor.UserId]--;

                            if (m_crossoverTaskNeighborCount[neighbor.UserId] < minTaskNeighbors ||
                                (m_crossoverTaskNeighborCount[neighbor.UserId] == minTaskNeighbors && m_rand.Next(2) == 0))
                            {
                                minTask = neighbor;
                                minTaskNeighbors = m_crossoverTaskNeighborCount[neighbor.UserId];
                            }
                        }
                    }

                    task = minTask;

                    if (task == null)
                    {
                        throw new Exception("null task");
                    }
                }
                else
                {
                    // find a task not already added...

                    for (int notAddedTaskIndex = 0; notAddedTaskIndex < TaskSequencer.getOriginalTasks().Count; notAddedTaskIndex++)
                    {
                        if (m_crossoverAddedTasks[notAddedTaskIndex] == false)
                        {
                            task = TaskSequencer.getOriginalTasks()[notAddedTaskIndex];
                            if (task == null)
                            {
                                throw new Exception("null task");
                            }

                            break;
                        }
                    }
                }
            }

            updateFitness();


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

        public void mutate()
        {
            int nbMutations = m_rand.Next(Tasks.Count / 5) + 1;
            for (int t = 0; t < nbMutations; t++)
            {
                int t1 = m_rand.Next(Tasks.Count);
                int range = (int)(m_rand.Next((int)(Tasks.Count * 0.3)) - Tasks.Count * 0.15 - 1);
                //int range = (int)(m_rand.Next(5) - 2);
                int t2 = (t1 + range) % Tasks.Count;
                if (t2 < 0)
                {
                    t2 = Tasks.Count + t2;
                }
                Task temp = Tasks[t1];
                Tasks[t1] = Tasks[t2];
                Tasks[t2] = temp;
            }

            updateFitness();
        }

        public void optimize()
        {
            // nothing to do!
        }

        private int m_id;
        private Robot m_robot;
        private FitnessLevels m_fitnessLevels;
        private int m_fitness = 66666666, m_time = 66666666, m_cost = 66666666;
        private List<Task> m_tasks;
        private int m_startX, m_startY;
        private Random m_rand = new Random();

        private Task[][] m_crossoverTaskNeighbors;
        private int[] m_crossoverTaskNeighborCount;
        private bool[][] m_crossoverTaskNeighborMatrix;
        private bool[] m_crossoverAddedTasks;

        private TaskSequencer m_sequencer = null;
    }
}
