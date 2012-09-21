using System;
using System.Collections.Generic;
using TaskOptimizer.Interfaces;

namespace TaskOptimizer.Model
{
    public class TaskDistribution : Individual
    {
        private Random _rand;

        private readonly TaskSequencerNearestInsert _nearestInsert;
        private List<Task>[] _distributedTasks;
        private TaskDistributor _distributor;
        private int[] _lastTaskDistributedWorker;

        private bool _optimizingSequences = false;
        private List<Worker> _workers;
        private TaskSequencer[] _sequencers;
        private int[] _taskDistributedWorker;
        private List<Task> _tasks;

        public TaskDistribution(Configuration config)
        {
            _nearestInsert = new TaskSequencerNearestInsert();
            Configure(config);
        }

        public TaskDistribution(TaskDistribution distribution)
        {
            _nearestInsert = new TaskSequencerNearestInsert();

            var config = new Configuration(distribution._workers, distribution._tasks, distribution._distributor, distribution._rand.Next());

            Console.WriteLine(config.Workers.Count);
            Id = distribution.Id;

            Configure(config);
            Console.WriteLine("Configured");
            Fitness = Int32.MaxValue;
            Console.WriteLine(distribution.Sequences.Count);
            SetSequences(distribution.Sequences);
        }

        public bool OptimizeSequences
        {
            set
            {
                foreach (TaskSequencer sequencer in _sequencers)
                {
                    if (value)
                    {
                        //TODO check if this is correct logic
                        _sequencers[0].UseOptimalPopulationSize();
                    }
                    else
                    {
                        //TODO check if this is correct logic
                        _sequencers[0].PopulationSize = 1;
                    }
                }

                _optimizingSequences = value;
            }

            get { return _optimizingSequences; }
        }

        public int NbUsedWorkers { get; private set; }

        public List<TaskSequence> Sequences { get; private set; }

        #region Individual Members

        public int Id { get; set; }

        public int Fitness { get; private set; }

        public void Optimize()
        {
            Fitness = 0;
            int usedWorkers = 0;

            for (int t = 0; t < _workers.Count; t++)
            {
                _sequencers[t].Optimize();

                TaskSequence sequence = _sequencers[t].MinTaskSequence;
                if (sequence != null)
                {
                    Fitness += sequence.Fitness;

                    if (sequence.Tasks.Count > 0)
                    {
                        usedWorkers++;
                    }
                }

                Sequences[t] = sequence;
            }

            NbUsedWorkers = usedWorkers;

            Fitness /= NbUsedWorkers;
        }

        /// <summary>
        /// We become the child of these 2 parent distributions
        /// </summary>        
        public void Crossover(Individual parent1, Individual parent2)
        {
            var p1 = parent1 as TaskDistribution;
            var p2 = parent2 as TaskDistribution;

            bool changed = false;

            for (int t = 0; t < _tasks.Count; t++)
            {
                int distributedWorkerBefore = _taskDistributedWorker[t];

                if (_rand.Next() % 2 == 0)
                {
                    _taskDistributedWorker[t] = p1._taskDistributedWorker[t];
                }
                else
                {
                    _taskDistributedWorker[t] = p2._taskDistributedWorker[t];
                }

                if (distributedWorkerBefore != _taskDistributedWorker[t])
                {
                    changed = true;
                }
            }

            if (changed)
            {
                AfterDistributionChanged();
            }
            else
            {
                // population surely is very similar... introduce some variations!
                Mutate();
            }
        }

        public void Mutate()
        {
            int force = _distributor.ComputeMutationForce();

            if (force > 10)
            {
                force = 10;
            }

            var nbMutations = (int)(_rand.Next(_tasks.Count / 10) * (force / 3.0) + 1);
            bool changed = false;
            double nearestTolerance = 1.0 + force / 12.0;
            for (int t = 0; t < nbMutations; t++)
            {
                int taskIndex = _rand.Next(_tasks.Count);

                Task nearestTask = _nearestInsert.FindAlmostNearestTask(_tasks[taskIndex], _tasks, nearestTolerance);
                if (_taskDistributedWorker[taskIndex] != _taskDistributedWorker[nearestTask.Id])
                {
                    // switch the task to a close worker
                    _taskDistributedWorker[taskIndex] = _taskDistributedWorker[nearestTask.Id];
                    changed = true;
                }
                else
                {
                    // assign a random worker...
                    _taskDistributedWorker[taskIndex] = _rand.Next(_workers.Count);
                    changed = true;
                }
            }

            if (changed)
            {
                AfterDistributionChanged();
            }
        }

        #endregion

        public void SetSequences(List<TaskSequence> sequences)
        {
            int workerIndex = 0;
            Console.WriteLine(sequences.Count);
            foreach (TaskSequence sequence in sequences)
            {
                if (sequence != null)
                {
                    foreach (Task task in sequence.Tasks)
                    {
                        _taskDistributedWorker[task.Id] = workerIndex;
                    }
                }
                workerIndex++;
            }

            UpdateDistributedTasksFromDistribution();

            for (int t = 0; t < _workers.Count; t++)
            {
                TaskSequencer.Configuration config;

                if (_distributedTasks[t].Count > 0 & sequences[t] != null)
                {
                    var tasks = CloneTaskList(sequences[t].Tasks);

                    config = new TaskSequencer.Configuration(_workers[t], tasks, (sequences[t]).Fitness, true);
                }
                else
                {
                    config = new TaskSequencer.Configuration(_workers[t], new List<Task>());
                }

                _sequencers[t].Configure(config);
            }

            Optimize();
        }

        public void GenerateInitialSolution()
        {
            int method = _rand.Next(10);

            if (method < 1)
            {
                GenerateFixedInitialSolution(_rand.Next(_workers.Count));
            }
            else if (method < 5)
            {
                GenerateRandomInitialSolution();
            }
            else
            {
                GenerateKMeanInitialSolution();
            }
        }


        /// <summary>
        /// All tasks for one worker
        /// </summary>        
        public void GenerateFixedInitialSolution(int workerIndex)
        {
            for (int t = 0; t < _tasks.Count; t++)
            {
                _taskDistributedWorker[t] = workerIndex;
            }

            AfterDistributionChanged();
        }

        public void RecomputeFitness()
        {
            foreach (TaskSequencer sequencer in _sequencers)
            {
                sequencer.RecomputeFitness();
            }

            Optimize();
        }

        public Task FindNearestAssignedTask(Task fromTask)
        {
            int minDistance = Int32.MaxValue;
            Task minTask = null;

            foreach (Task task in _tasks)
            {
                // assigned?
                if (_taskDistributedWorker[task.Id] != -1 && fromTask.Id != task.Id)
                {
                    int distance = task.DistanceTo(fromTask);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minTask = task;
                    }
                }
            }


            return minTask;
        }

        private void Configure(Configuration config)
        {
            _rand = new Random(config.RandomSeed);

            ConfigureTasks(config);

            _workers = config.Workers;
            _distributor = config.Distributor;

            _taskDistributedWorker = new int[_tasks.Count];
            _lastTaskDistributedWorker = new int[_tasks.Count];
            _sequencers = new TaskSequencer[_workers.Count];
            _distributedTasks = new List<Task>[_workers.Count];
            Sequences = new List<TaskSequence>(_workers.Count);

            for (int t = 0; t < _workers.Count; t++)
            {
                _distributedTasks[t] = new List<Task>(_tasks.Count);
                _sequencers[t] = new TaskSequencer(_tasks.Count);
                Sequences.Add(null);
            }

            for (int t = 0; t < _tasks.Count; t++)
            {
                _lastTaskDistributedWorker[t] = -1;
            }
        }

        private void ConfigureTasks(Configuration config)
        {
            _tasks = CloneTaskList(config.Tasks);
        }

        private static List<Task> CloneTaskList(List<Task> tasks)
        {
            var clonedTasks = new List<Task>(tasks.Count);
            foreach (Task task in tasks)
            {
                clonedTasks.Add(new Task(task));
            }

            return clonedTasks;
        }

        private void GenerateKMeanInitialSolution()
        {
            if (_workers.Count < 2)
            {
                // no need to cluster!
                GenerateFixedInitialSolution(0);
                return;
            }

            var clusterEngine = new KMeanCluster();
            // int nbWorkers = _rand.Next(_workers.Count * 2);
            // if (nbWorkers < 2)
            //{
            //    nbWorkers = 2;
            // }

            // if (nbWorkers > _workers.Count)
            //{
            int nbWorkers = _workers.Count;
            //}

            clusterEngine.Compute(_tasks, nbWorkers, _taskDistributedWorker);
            AfterDistributionChanged();
        }

        private void GenerateRandomInitialSolution()
        {
            if (_workers.Count < 2)
            {
                // no need for random!
                GenerateFixedInitialSolution(0);
                return;
            }

            var remainingTasks = new List<Task>(_tasks);

            int workerIndex = _rand.Next(_workers.Count);

            var maxTasksPerWorker = (int)(_tasks.Count / (double)_workers.Count);

            for (int t = 0; t < _workers.Count; t++)
            {
                int nbTasks = maxTasksPerWorker;

                if (nbTasks > remainingTasks.Count)
                {
                    nbTasks = remainingTasks.Count;
                }

                if (t == _workers.Count - 1)
                {
                    nbTasks = remainingTasks.Count;
                }

                if (nbTasks > 0)
                {
                    TaskSequence sequence = _nearestInsert.Generate(remainingTasks, nbTasks, _workers[workerIndex]);

                    foreach (Task task in sequence.Tasks)
                    {
                        _taskDistributedWorker[task.Id] = workerIndex;
                    }
                }

                workerIndex = (workerIndex + 1) % _workers.Count;
            }

            AfterDistributionChanged();
        }

        private bool IsLastDistributionEqualsNewOne()
        {
            bool changed = false;
            for (int t = 0; t < _lastTaskDistributedWorker.Length; t++)
            {
                if (_lastTaskDistributedWorker[t] != _taskDistributedWorker[t])
                {
                    changed = true;
                    break;
                }
            }

            return !changed;
        }

        private void AfterDistributionChanged()
        {
            if (IsLastDistributionEqualsNewOne())
            {
                return;
            }

            UpdateDistributedTasksFromDistribution();

            ConfigureSequencersFromDistribution();
        }

        private void UpdateDistributedTasksFromDistribution()
        {
            ClearDistributedTasks();

            for (int t = 0; t < _tasks.Count; t++)
            {
                _distributedTasks[_taskDistributedWorker[t]].Add(_tasks[t]);

                _lastTaskDistributedWorker[t] = _taskDistributedWorker[t];
            }
        }

        private void ClearDistributedTasks()
        {
            for (int t = 0; t < _workers.Count; t++)
            {
                _distributedTasks[t].Clear();
            }
        }

        private void ConfigureSequencersFromDistribution()
        {
            for (int t = 0; t < _workers.Count; t++)
            {
                var config = new TaskSequencer.Configuration( _workers[t], _distributedTasks[t]);
                _sequencers[t].Configure(config);
            }

            Optimize();
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public List<Task> Tasks { get; private set; }
            public List<Worker> Workers { get; private set; }

            public TaskDistributor Distributor { get; private set; }
            public int RandomSeed { get; private set; }

            public Configuration(List<Worker> workers, List<Task> tasks = null, TaskDistributor distributor = null, int randomSeed = 0)
            {
                Tasks = tasks;
                Workers = workers;

                Distributor = distributor;
                RandomSeed = randomSeed;
            }
        }

        #endregion
    }
}