using System;
using System.Collections.Generic;

namespace TaskOptimizer.Model
{
    internal class KMeanCluster
    {
        private readonly Random _rand = new Random();
        private List<List<Task>> _meanTasks;
        private List<Task> _means;
        private int _overallDistanceToMeans = Int32.MaxValue;
        private int[] _taskAssignedCluster;
        private List<Task> _tasks;

        public void Compute(List<Task> tasks, int nbClusters, int[] taskAssignedCluster)
        {
            if (taskAssignedCluster.Length < tasks.Count)
            {
                throw new Exception("Not enough space in the taskAssignedCluster array");
            }

            if (tasks.Count < nbClusters)
            {
                nbClusters = tasks.Count;
            }

            _tasks = tasks;
            _taskAssignedCluster = taskAssignedCluster;
            _means = new List<Task>(nbClusters);

            _meanTasks = new List<List<Task>>(nbClusters);
            for (int t = 0; t < nbClusters; t++)
            {
                _meanTasks.Add(new List<Task>(tasks.Count/nbClusters + 1));
            }

            ClearTaskAssignedCluster();
            SelectRandomMeans(nbClusters);
            AssignTasksToNearestMean();

            int lastOverallDistanceToMeans;
            do
            {
                lastOverallDistanceToMeans = _overallDistanceToMeans;
                Compute();
            } while (_overallDistanceToMeans < lastOverallDistanceToMeans);
        }

        private void Compute()
        {
            SelectNextMeans();
            AssignTasksToNearestMean();
        }

        private void AssignTasksToNearestMean()
        {
            foreach (var task in _tasks)
            {
                int meanIndex = FindNearestMean(task);
                _taskAssignedCluster[task.Id] = meanIndex;
                _meanTasks[meanIndex].Add(task);
            }
        }

        private void ClearTaskAssignedCluster()
        {
            for (int t = 0; t < _tasks.Count; t++)
            {
                _taskAssignedCluster[t] = -1;
            }
        }

        private int FindNearestMean(Task task)
        {
            int minDistance = Int32.MaxValue;
            int minIndex = -1;
            int index = 0;

            foreach (Task mean in _means)
            {
                int cost = task.Problem.CostFunction.Calculate(task, mean, false);
                if (cost < minDistance)
                {
                    minIndex = index;
                    minDistance = cost;
                }

                index++;
            }

            return minIndex;
        }

        private void SelectNextMeans()
        {
            _means.Clear();

            _overallDistanceToMeans = 0;

            foreach (var tasks in _meanTasks)
            {
                int minTotalDistance = 0;
                Task minTask = FindCentroidTask(tasks, out minTotalDistance);

                _overallDistanceToMeans += minTotalDistance;

                tasks.Clear();
                _means.Add(minTask);
            }
        }

        private void SelectRandomMeans(int nbClusters)
        {
            _means.Clear();

            while (_means.Count < nbClusters)
            {
                var mean = _tasks[_rand.Next(_tasks.Count)];
                if (_taskAssignedCluster[mean.Id] == -1)
                    _means.Add(mean);
            }
        }

        /// <summary>
        /// The task closest to all other tasks.
        /// Using straight distance instead of route calculation for efficiency, should end up with the same result most of the time.
        /// Don't assume euclidean distance to be able to work with asymetrical values.
        /// </summary>        
        private static Task FindCentroidTask(List<Task> tasks, out int distance)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (var fromTask in tasks)
            {
                distance = 0;

                foreach (var toTask in tasks)
                {
                    //TODO use cached table
                    //distance += fromTask.StraightDistanceTo(toTask);
                    distance += fromTask.Problem.CostFunction.Calculate(fromTask, toTask, false);
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minTask = fromTask;
                }
            }

            distance = minDistance;
            return minTask;
        }
    }
}