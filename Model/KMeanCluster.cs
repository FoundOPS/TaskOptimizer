using System;
using System.Collections.Generic;
using System.Text;

namespace TaskOptimizer.Model
{
    class KMeanCluster
    {
        public void compute(List<Task> tasks, int nbClusters, int[] taskAssignedCluster)
        {
            if (taskAssignedCluster.Length < tasks.Count)
            {
                throw new Exception("Not enough space in the taskAssignedCluster array");
            }
                        
            if (tasks.Count < nbClusters)
            {
                nbClusters = tasks.Count;
            }

            m_tasks = tasks;
            m_taskAssignedCluster = taskAssignedCluster;
            m_means = new List<Task>(nbClusters);

            m_meanTasks = new List<List<Task>>(nbClusters);
            for (int t = 0; t < nbClusters; t++)
            {
                m_meanTasks.Add(new List<Task>(tasks.Count / nbClusters + 1));
            }

            clearTaskAssignedCluster();
            selectRandomMeans(nbClusters);
            assignTasksToNearestMean();

            int lastOverallDistanceToMeans;
            do
            {
                lastOverallDistanceToMeans = m_overallDistanceToMeans;
                compute();

            } while (m_overallDistanceToMeans < lastOverallDistanceToMeans);
            
        }

        private void clearTaskAssignedCluster()
        {            
            for(int t=0; t<m_tasks.Count; t++)
            {
                m_taskAssignedCluster[t] = -1;                
            }           
        }

        private void selectRandomMeans(int nbClusters)
        {
            m_means.Clear();

            while(m_means.Count < nbClusters)
            {
                Task mean = m_tasks[m_rand.Next(m_tasks.Count)];
                if (m_taskAssignedCluster[mean.Id] == -1)
                {
                    m_means.Add(mean);
                    int meanIndex = m_means.Count - 1;
                }
            }
        }

        private void compute()
        {
            selectNextMeans();
            assignTasksToNearestMean();
        }

        private void assignTasksToNearestMean()
        {
            foreach(Task task in m_tasks)
            {
                int meanIndex = findNearestMean(task);
                m_taskAssignedCluster[task.Id] = meanIndex;
                m_meanTasks[meanIndex].Add(task);
            }                        
        }

        private int findNearestMean(Task task)
        {
            int minDistance = Int32.MaxValue;
            int minIndex = -1;
            int index = 0;

            foreach(Task mean in m_means)
            {
                int distance = task.distanceTo(mean);
                if( distance < minDistance)
                {
                    minIndex = index;
                    minDistance = distance;
                }

                index++;
            }

            return minIndex;
        }

        private void selectNextMeans()
        {            
            m_means.Clear();

            m_overallDistanceToMeans = 0;
            int minTotalDistance = 0;
            Task minTask = null;

            foreach (List<Task> tasks in m_meanTasks)
            {
                minTask = findCentroidTask(tasks, out minTotalDistance);
                
                m_overallDistanceToMeans += minTotalDistance; 

                tasks.Clear();                                
                m_means.Add(minTask);
            }
        }

        /// <summary>
        /// The task closest to all other tasks. Don't assume euclidean distance to be able to work
        /// with asymetrical values.
        /// </summary>        
        private Task findCentroidTask(List<Task> tasks, out int distance)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (Task fromTask in tasks)
            {
                distance = 0;

                foreach (Task toTask in tasks)
                {
                    distance += fromTask.distanceTo(toTask);
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

        private int m_overallDistanceToMeans = Int32.MaxValue;
        private List<List<Task>> m_meanTasks;
        private int[] m_taskAssignedCluster;
        private List<Task> m_means;
        private List<Task> m_tasks;
        private Random m_rand = new Random();
    }
}
