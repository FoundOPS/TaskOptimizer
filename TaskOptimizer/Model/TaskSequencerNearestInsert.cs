using System;
using System.Collections.Generic;

namespace TaskOptimizer.Model
{
    internal class TaskSequencerNearestInsert
    {
        private readonly Random _rand = new Random(10);

        public void Generate(List<Task> remainingTasks, int nbTasksInSequence, TaskSequence sequence)
        {
            var orderedTasks = new List<Task>(nbTasksInSequence);

            if (nbTasksInSequence > remainingTasks.Count)
            {
                throw new Exception("Not enough tasks to generate the specified sequence length");
            }

            if (remainingTasks.Count > 0)
            {
                var task = FindAlmostNearestTask(null, remainingTasks, 1.0);
                remainingTasks.Remove(task);
                orderedTasks.Add(task);

                if (remainingTasks.Count > 0)
                {
                    task = FindAlmostNearestTask(task, remainingTasks, 1.0);
                    remainingTasks.Remove(task);
                    orderedTasks.Add(task);

                    orderedTasks.Add(null);

                    for (int t = 0; t < nbTasksInSequence - 2; t++)
                    {
                        int minCost = Int32.MaxValue;
                        Task minTask = null;
                        int minInsertPos = -1;

                        for (int t1 = 0; t1 < orderedTasks.Count - 1; t1++)
                        {
                            int cost = FindCheapestInsert(orderedTasks[t1], orderedTasks[t1 + 1], remainingTasks, out task);
                            if (cost < minCost)
                            {
                                minCost = cost;
                                minTask = task;
                                minInsertPos = t1;
                            }
                        }

                        remainingTasks.Remove(minTask);
                        orderedTasks.Insert(minInsertPos + 1, minTask);
                    }

                    orderedTasks.RemoveAt(orderedTasks.Count - 1);
                }
            }

            sequence.Tasks = orderedTasks;
        }

        public TaskSequence Generate(List<Task> remainingTasks, int nbTasksInSequence, Worker worker)
        {
            var sequence = new TaskSequence{ Worker = worker};

            Generate(remainingTasks, nbTasksInSequence, sequence);

            return sequence;
        }

        private int FindCheapestInsert(Task t1, Task t2, IEnumerable<Task> remainingTasks, out Task newTask)
        {
            int minCost = Int32.MaxValue;
            int constantCost = t1.DistanceTo(t2);
            newTask = null;

            foreach (Task task in remainingTasks)
            {
                int cost = t1.DistanceTo(task) + task.DistanceTo(t2);
                if (cost < minCost)
                {
                    minCost = cost;
                    newTask = task;
                }
            }

            return minCost - constantCost;
        }

        public Task FindAlmostNearestTask(Task fromTask, List<Task> tasks, double tolerance)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (Task task in tasks)
            {
                int distance = task.DistanceTo(fromTask);
                if (distance < minDistance && (minTask == null || _rand.Next(2) == 0))
                {
                    var adjustedDistance = (int)(distance * tolerance);
                    if (adjustedDistance < minDistance)
                    {
                        minDistance = adjustedDistance;
                    }
                    minTask = task;
                }
            }

            return minTask;
        }
    }
}