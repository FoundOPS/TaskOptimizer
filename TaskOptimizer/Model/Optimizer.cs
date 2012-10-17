using System;
using System.Collections.Generic;
using System.Threading;
using TaskOptimizer.Calculator;

namespace TaskOptimizer.Model
{
    public class Optimizer
    {
        public Thread CreationThread;
        private int _mCurIteration;
        private TaskDistributor[] _mDistributors;
        private bool _mEndOptimizeThread;

        public TaskDistributor MinDistributor;
        private int _minDistributorFitness = Int32.MaxValue;
        private Random _rand;
        private bool[] _recomputeFitnesses;
        private Thread[] _threads;

        public Problem Problem { get; private set; }


        public Optimizer(Problem p, Configuration config)
        {
            Problem = p;
            Create(config);
        }

        public int CurrentIteration
        {
            get { return _mCurIteration; }
        }

        public List<TaskSequence> MinSequences
        {
            get { return new List<TaskSequence>(MinDistributor.MinSequences); }
        }

        public int Fitness
        {
            get { return MinDistributor.Fitness; }
        }

        public Boolean StillInit()
        {
            if (CreationThread.ThreadState != ThreadState.Stopped) return true;
            return false;
        }

        private void Create(Configuration config)
        {
            _rand = new Random(config.RandomSeed);

            _mDistributors = new TaskDistributor[config.NumberDistributors];

            _threads = new Thread[config.NumberDistributors];
            _recomputeFitnesses = new bool[config.NumberDistributors];

            var taskDistributorConfiguration = new TaskDistributor.Configuration {Workers = config.Workers, Tasks = config.Tasks, Optimizer = this};

            for (int t = 0; t < config.NumberDistributors; t++)
            {
                Console.WriteLine(t);

                taskDistributorConfiguration.RandomSeed = _rand.Next();
                taskDistributorConfiguration.StartProgressPercent = t*100/config.NumberDistributors;
                taskDistributorConfiguration.EndProgressPercent = taskDistributorConfiguration.StartProgressPercent + 100/config.NumberDistributors;
                _mDistributors[t] = new TaskDistributor(taskDistributorConfiguration);

                _recomputeFitnesses[t] = false;
                _threads[t] = new Thread(OptimizeThread);
            }

            Start();
        }

        public void RecomputeFitness()
        {
            for (int t = 0; t < _recomputeFitnesses.Length; t++)
            {
                _recomputeFitnesses[t] = true;
            }
        }

        private void Start()
        {
            Compute();
            _mEndOptimizeThread = false;
            for (int t = 0; t < _threads.Length; t++)
            {
                _threads[t].Start(t);
            }
        }

        /// <summary>
        /// Update the min distributor
        /// </summary>
        public void Compute()
        {
            int nbIterations = 0;
            for (int t = 0; t < _mDistributors.Length; t++)
            {
                nbIterations += _mDistributors[t].CurrentIteration;
                if (_mDistributors[t].Fitness < _minDistributorFitness)
                {
                    _minDistributorFitness = _mDistributors[t].Fitness;
                    MinDistributor = _mDistributors[t];
                }
            }

            _mCurIteration = nbIterations;
        }

        public void Stop()
        {
            _mEndOptimizeThread = true;
            foreach (Thread t in _threads)
            {
                if (t != null && t.IsAlive)
                   t.Join();
            }
        }

        private void OptimizeThread(object index)
        {
            var distributorIndex = (int) index;

            TaskDistributor distributor = _mDistributors[distributorIndex];

            while (!_mEndOptimizeThread)
            {
                if (_recomputeFitnesses[distributorIndex])
                {
                    distributor.RecomputeFitness();
                    _recomputeFitnesses[distributorIndex] = false;
                }
                distributor.Optimize();

                Console.WriteLine("Distributor #{0,-2} Optimized! Fitness = {1}", index, distributor.Fitness);
            }
        }

        #region Nested type: Configuration

        public class Configuration
        {
            public int NumberDistributors { get; set; }
            public int RandomSeed { get; set; }
            public List<Worker> Workers { get; set; }
            public List<Task> Tasks { get; set; }
        }

        #endregion
    }
}