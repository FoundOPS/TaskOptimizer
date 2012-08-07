using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TaskOptimizer.Model
{
    class TaskSequencerNearestInsert
    {
        public void generate(List<Task> remainingTasks, int nbTasksInSequence, TaskSequence sequence)
        {            
            List<Task> ordered_tasks = new List<Task>(nbTasksInSequence);

            if (nbTasksInSequence > remainingTasks.Count)
            {
                throw new Exception("Not enough tasks to generate the specified sequence length");
            }

            if (remainingTasks.Count > 0)
            {
                Task task = findAlmostNearestTask(null, remainingTasks, 1.0);
                remainingTasks.Remove(task);
                ordered_tasks.Add(task);

                if (remainingTasks.Count > 0)
                {

                    task = findAlmostNearestTask(task, remainingTasks, 1.0);
                    remainingTasks.Remove(task);
                    ordered_tasks.Add(task);

                    ordered_tasks.Add(null);

                    for (int t = 0; t < nbTasksInSequence - 2; t++)
                    {
                        int minCost = Int32.MaxValue;
                        Task minTask = null;
                        int minInsertPos = -1;

                        for (int t1 = 0; t1 < ordered_tasks.Count - 1; t1++)
                        {
                            int cost = findCheapestInsert(ordered_tasks[t1], ordered_tasks[t1 + 1], remainingTasks, out task);
                            if (cost < minCost)
                            {
                                minCost = cost;
                                minTask = task;
                                minInsertPos = t1;
                            }
                        }
                       
                        remainingTasks.Remove(minTask);
                        ordered_tasks.Insert(minInsertPos + 1, minTask);
                    }

                    ordered_tasks.RemoveAt(ordered_tasks.Count - 1);
                }
            }

            sequence.Tasks = ordered_tasks;  
        }

        public TaskSequence generate(List<Task> remainingTasks, int nbTasksInSequence, Robot robot, int startX, int startY, FitnessLevels fitnessLevels)
        {            
            TaskSequence sequence = new TaskSequence();

            sequence.Robot = robot;
            sequence.StartX = startX;
            sequence.StartY = startY;
            sequence.FitnessLevels = fitnessLevels;

            generate(remainingTasks, nbTasksInSequence, sequence);

            return sequence;
        }

        private int findCheapestInsert(Task t1, Task t2, List<Task> remainingTasks, out Task newTask)
        {
            int minCost = Int32.MaxValue;
            int constantCost = t1.distanceTo(t2);
            newTask = null;

            foreach (Task task in remainingTasks)
            {
                int cost = t1.distanceTo(task) + task.distanceTo(t2);
                if (cost < minCost)
                {
                    minCost = cost;
                    newTask = task;
                }
            }

            return minCost - constantCost;
        }

        public Task findAlmostNearestTask(Task fromTask, List<Task> tasks, double tolerance)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (Task task in tasks)
            {
                int distance = task.distanceTo(fromTask);
                if (distance < minDistance && (minTask == null || m_rand.Next(2) == 0))
                {
                    int adjustedDistance = (int)(distance * tolerance);
                    if (adjustedDistance < minDistance)
                    {
                        minDistance = adjustedDistance;
                    }
                    minTask = task;
                }
            }            

            return minTask;

        }

        private Random m_rand = new Random(10);
    }
}
