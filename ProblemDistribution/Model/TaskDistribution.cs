using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.DataModel;
using ProblemLib.Interfaces;

namespace ProblemDistribution.Model
{
    // Work in progress therefore not commented

    class TaskDistribution : Individual, ICloneable
    {
        private DistributionOptimizer optimizer;

        private Random rand;

        private List<Task> tasks; 
        private List<Worker> workers;
        private List<Task>[] distributedTasks;
        private TaskSequencer[] sequencers;
        private readonly NearestInsertSequencer nearestInsert;
        private Int32[] taskDistributedWorker;
        private Int32[] lastTaskDistributedWorker;
        private Boolean optimizingSequences = false;



        public Boolean OptimizeSequences
        {
            get { return optimizingSequences; }
            set
            {
                // TODO Useless Loop?? Cleanup this mess!!
                foreach (TaskSequencer seq in sequencers)
                {
                    if (value) sequencers[0].UseOptimalPopuationSize();
                    else sequencers[0].PopulationSize = 1;
                }
                optimizingSequences = value;
            }
        }

        public List<TaskSequence> Sequences { get; private set; } 


        public TaskDistribution(DistributionOptimizer optimizer)
        {
            this.optimizer = optimizer;
            nearestInsert = new NearestInsertSequencer(optimizer);
        }

        public void Initialize(List<Worker> workers, List<Task> tasks, Int32 randomSeed)
        {
            rand = new Random(randomSeed);

            // Configure tasks and workers
            this.tasks = CloneTaskList(tasks);
            this.workers = workers;

            taskDistributedWorker = new Int32[tasks.Count];
            lastTaskDistributedWorker = new Int32[tasks.Count];
            sequencers = new TaskSequencer[workers.Count];
            distributedTasks = new List<Task>[workers.Count];
            Sequences = new List<TaskSequence>(workers.Count);

            for (int i = 0; i < workers.Count; i++)
            {
                distributedTasks[i] = new List<Task>(tasks.Count);
                sequencers[i] = new TaskSequencer(tasks.Count);
                Sequences.Add(null);
            }

            for (int i = 0; i < tasks.Count; i++)
                lastTaskDistributedWorker[i] = -1;
        }



        /// <summary>
        /// Generates initial solution
        /// </summary>
        /// <param name="workerIndex">Index of the worker that will receive ALL tasks; -1 for K-Mean or Random solution</param>
        /// <remarks>
        /// This method combines GenerateFixedInitialSolution() and GenerateInitialColution() of the TaskOptimizer.TaskDistributor class.
        /// If workerIndex is not -1, all tasks will be assigned to that worker. Otherwise calling this method is equivalent to calling
        /// TaskOptimizer.TaskDistributor.GenerateInitialSolution().
        /// </remarks>
        public void GenerateInitialSolution(Int32 workerIndex = -1)
        {
            throw new NotImplementedException();
        }

        public void UpdateFitness()
        {
            throw new NotImplementedException();
        }

        #region Members of Individual

        public int Id { get; set; }

        public int Fitness
        {
            get { throw new NotImplementedException(); }
        }

        public void Optimize()
        {
            throw new NotImplementedException();
        }

        public void Crossover(Individual parent1, Individual parent2)
        {
            throw new NotImplementedException();
        }

        public void Mutate()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Members of ICloneable

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utility Methods

        private static List<Task> CloneTaskList(List<Task> original)
        {
            List<Task> cloned = new List<Task>(original.Count);
            foreach (Task task in original)
                cloned.Add(task);
            return cloned;
        }

        #endregion
    }
}
