using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.DataModel;

namespace ProblemDistribution.Model
{
    /// <summary>
    /// Generates optimal task orders by finding the most optimal insert points
    /// </summary>
    class NearestInsertSequencer
    {
        private readonly Random rand = new Random(10); // Why a constant random seed ???
        private readonly DistributionOptimizer optimizer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="optimizer">DistributionOptimizer that this seuencer belongs to</param>
        public NearestInsertSequencer(DistributionOptimizer optimizer)
        {
            this.optimizer = optimizer;
        }

        /// <summary>
        /// Generates a task sequence from remainin tasks and assigns it to the specified
        /// sequence.
        /// </summary>
        /// <param name="remaining">List of remaining tasks</param>
        /// <param name="tasksInSequence">Number of tasks in a sequence</param>
        /// <param name="sequence">TaskSequence o assign tasks to</param>
        public void Generate(List<Task> remaining, Int32 tasksInSequence, TaskSequence sequence)
        {
            HashSet<Task> tmpRemaining = new HashSet<Task>(remaining);
            List<Task> orderedTasks = new List<Task>(tasksInSequence);

            if (tasksInSequence > remaining.Count)
                throw new Exception("Not enough tasks to fill the sequence!");

            if (remaining.Count > 0)
            {
                Task task = FindAlmostNearestTask(null, tmpRemaining, 1.0);
                remaining.Remove(task); // bad way to do it, need a better way
                orderedTasks.Add(task);

                if (remaining.Count > 0)
                {
                    task = FindAlmostNearestTask(task, tmpRemaining, 1.0);
                    remaining.Remove(task); // same as above
                    orderedTasks.Add(task);

                    orderedTasks.Add(null);

                    for (int i = 0; i < tasksInSequence; i++)
                    {
                        Int32 minCost = Int32.MaxValue;
                        Task minTask = null;
                        int minInsertPos = -1;

                        for (int j = 0; j < orderedTasks.Count - 1; j++)
                        {
                            int cost = FindCheapestInsert(orderedTasks[j], orderedTasks[j + 2], remaining, out task);
                            if (cost < minCost)
                            {
                                minCost = cost;
                                minTask = task;
                                minInsertPos = j;
                            }
                        }

                        remaining.Remove(minTask);
                        orderedTasks.Insert(minInsertPos + 1, minTask);
                    }

                    orderedTasks.RemoveAt(orderedTasks.Count - 1);
                }
            }

            sequence.Tasks = orderedTasks;
        }
        /// <summary>
        /// Generates a task sequence from remaining tasks and assigns it to a new sequence.
        /// </summary>
        /// <param name="remaining">List of remaining tasks</param>
        /// <param name="tasksInSequence">Number of tasks in a sequence</param>
        /// <param name="w">Worker to assign tasks to</param>
        /// <returns></returns>
        public TaskSequence Generate(List<Task> remaining, Int32 tasksInSequence, Worker w)
        {
            TaskSequence sequence = new TaskSequence(optimizer) { Worker = w };
            Generate(remaining, tasksInSequence, sequence);
            return sequence;
        }

        /// <summary>
        /// Find the best possible task to insert between two existing tasks.
        /// </summary>
        /// <param name="task0">First existing rask</param>
        /// <param name="task1">Second existing task</param>
        /// <param name="remaining">Collection of remaining tasks</param>
        /// <param name="insert">Result</param>
        /// <returns>Cost increase after insert</returns>
        /// <remarks>
        /// This method is a part of O(N^2) algorithm, improvememnt is desirable
        /// </remarks>
        private Int32 FindCheapestInsert(Task task0, Task task1, IEnumerable<Task> remaining, out Task insert)
        {
            Int32 minCost = Int32.MaxValue;
            Int32 constCost = optimizer.CostFunction.Calculate(task0, task1, true);
            insert = null;

            foreach (Task task in remaining)
            {
                int cost = optimizer.CostFunction.Calculate(task0, task, true) + optimizer.CostFunction.Calculate(task, task1, true);
                if (cost < minCost)
                {
                    minCost = cost;
                    insert = null;
                }
            }

            return minCost - constCost;
        }
        /// <summary>
        /// Find a task that is near the origin task, but not necessarily the nearest.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="tasks"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Task FindAlmostNearestTask(Task origin, HashSet<Task> tasks, Double tolerance)
        {
            int minCost = Int32.MaxValue;
            Task minTask = null;

            foreach (Task task in tasks)
            {
                if (origin.TaskID == task.TaskID)
                    continue; // Skip the origin task if encountered

                Int32 cost = optimizer.CostFunction.Calculate(task, origin, true);
                if (cost < minCost && (minTask == null || rand.Next(2) == 0))
                {
                    Int32 adjCost = (Int32)(cost * tolerance);
                    if (adjCost < minCost)
                        minCost = adjCost;
                    minTask = task;
                }
            }

            return minTask;
        }
    }
}
