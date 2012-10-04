using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskSequence : Individual
    {
        private readonly Random _rand = new Random();
        private bool[] _crossoverAddedTasks;
        private int[] _crossoverTaskNeighborCount;
        private bool[][] _crossoverTaskNeighborMatrix;
        private Task[][] _crossoverTaskNeighbors;

        public Worker Worker { get; set; }
  
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get { return _tasks; }

            set
            {
                _tasks = value;
                ConfigureCrossoverData();
                UpdateFitness();
            }
        }

        public TaskSequencer TaskSequencer { set; get; }

        #region Individual Members

        public int Id { get; set; }

        private int _fitness = 66666666;
        public int Fitness
        {
            get { return _fitness; }

            set { _fitness = value; }
        }

        public void Crossover(Individual parent1, Individual parent2)
        {
            var p1 = parent1 as TaskSequence;
            var p2 = parent2 as TaskSequence;

            int beforeFitness = Fitness;

            ComputeEdgeRecombinaisonCrossover(p1, p2);

            if (beforeFitness == Fitness)
            {
                // no change? means that the parents are very close, so introduce some variety!
                Mutate();
            }
        }

        public void Mutate()
        {
            int nbMutations = _rand.Next(Tasks.Count / 5) + 1;
            for (int t = 0; t < nbMutations; t++)
            {
                int t1 = _rand.Next(Tasks.Count);
                var range = (int)(_rand.Next((int)(Tasks.Count * 0.3)) - Tasks.Count * 0.15 - 1);
                //int range = (int)(_rand.Next(5) - 2);
                int t2 = (t1 + range) % Tasks.Count;
                if (t2 < 0)
                {
                    t2 = Tasks.Count + t2;
                }
                Task temp = Tasks[t1];
                Tasks[t1] = Tasks[t2];
                Tasks[t2] = temp;
            }

            UpdateFitness();
        }

        public void Optimize()
        {
            // nothing to do!
        }

        #endregion

        private void ConfigureCrossoverData()
        {
            int maxTasks = _tasks.Count;

            if (_crossoverAddedTasks != null && _crossoverAddedTasks.Length >= maxTasks)
            {
                // enough space!
                return;
            }

            _crossoverAddedTasks = new bool[maxTasks];
            _crossoverTaskNeighborMatrix = new bool[maxTasks][];
            _crossoverTaskNeighbors = new Task[maxTasks][];
            _crossoverTaskNeighborCount = new int[maxTasks];
            for (int t = 0; t < maxTasks; t++)
            {
                _crossoverTaskNeighborMatrix[t] = new bool[maxTasks];
                _crossoverTaskNeighbors[t] = new Task[4];
            }
        }

        public void UpdateFitness()
        {
            int cost = 0;
            Task fromTask = null;

            foreach (var task in Tasks)
            {
                //do not calculate the cost from a task to itself
                if (task == fromTask)
                    continue;
                
                cost += task.CostTo(fromTask);
                fromTask = task;
            }
            
            Fitness = cost;
        }

        private void ComputeEdgeRecombinaisonCrossover(TaskSequence p1, TaskSequence p2)
        {
            // use the Edge recombination operator 
            // see: http://en.wikipedia.org/wiki/Edge_recombination_operator
            //_rand = new Random(11);
            for (int t = 0; t < TaskSequencer.GetOriginalTasks().Count; t++)
            {
                for (int t2 = 0; t2 < TaskSequencer.GetOriginalTasks().Count; t2++)
                {
                    _crossoverTaskNeighborMatrix[t][t2] = false;
                }
                _crossoverAddedTasks[t] = false;
                _crossoverTaskNeighborCount[t] = 0;

                _crossoverTaskNeighbors[t][0] = null;
                _crossoverTaskNeighbors[t][1] = null;
                _crossoverTaskNeighbors[t][2] = null;
                _crossoverTaskNeighbors[t][3] = null;
            }

            Tasks.Clear();

            AddNeighborsToCrossoverList(p1);
            AddNeighborsToCrossoverList(p2);
            Task task = null;

            if (_rand.Next(2) == 0)
            {
                task = p1.Tasks[0];
            }
            else
            {
                task = p2.Tasks[0];
            }

            for (int i = 0; i < TaskSequencer.GetOriginalTasks().Count; i++)
            {
                Tasks.Add(task);
                _crossoverAddedTasks[task.UserId] = true;

                int nbNeighbors = _crossoverTaskNeighborCount[task.UserId];

                if (nbNeighbors > 0)
                {
                    // find the neighbor with the fewest neighbors...

                    Task minTask = null;
                    int minTaskNeighbors = 99999;

                    // remove the task from its neighbors...
                    for (int t = 0; t < 4; t++)
                    {
                        Task neighbor = _crossoverTaskNeighbors[task.UserId][t];

                        if (neighbor != null && _crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId])
                        {
                            _crossoverTaskNeighborMatrix[neighbor.UserId][task.UserId] = false;
                            _crossoverTaskNeighborMatrix[task.UserId][neighbor.UserId] = false;
                            _crossoverTaskNeighborCount[neighbor.UserId]--;

                            if (_crossoverTaskNeighborCount[neighbor.UserId] < minTaskNeighbors ||
                                (_crossoverTaskNeighborCount[neighbor.UserId] == minTaskNeighbors &&
                                 _rand.Next(2) == 0))
                            {
                                minTask = neighbor;
                                minTaskNeighbors = _crossoverTaskNeighborCount[neighbor.UserId];
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

                    for (int notAddedTaskIndex = 0;
                         notAddedTaskIndex < TaskSequencer.GetOriginalTasks().Count;
                         notAddedTaskIndex++)
                    {
                        if (_crossoverAddedTasks[notAddedTaskIndex] == false)
                        {
                            task = TaskSequencer.GetOriginalTasks()[notAddedTaskIndex];
                            if (task == null)
                            {
                                throw new Exception("null task");
                            }

                            break;
                        }
                    }
                }
            }

            UpdateFitness();
        }

        private void AddNeighborsToCrossoverList(TaskSequence sequence)
        {
            Task prevTask = null;
            foreach (Task task in sequence.Tasks)
            {
                if (prevTask != null)
                {
                    if (_crossoverTaskNeighborMatrix[task.UserId][prevTask.UserId] == false)
                    {
                        _crossoverTaskNeighborMatrix[task.UserId][prevTask.UserId] = true;
                        _crossoverTaskNeighbors[task.UserId][_crossoverTaskNeighborCount[task.UserId]] = prevTask;
                        _crossoverTaskNeighborCount[task.UserId]++;
                    }

                    if (_crossoverTaskNeighborMatrix[prevTask.UserId][task.UserId] == false)
                    {
                        _crossoverTaskNeighborMatrix[prevTask.UserId][task.UserId] = true;
                        _crossoverTaskNeighbors[prevTask.UserId][_crossoverTaskNeighborCount[prevTask.UserId]] = task;
                        _crossoverTaskNeighborCount[prevTask.UserId]++;
                    }
                }

                prevTask = task;
            }
        }
    }
}