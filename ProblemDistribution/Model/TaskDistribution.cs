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

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public int Fitness
        {
            get { throw new NotImplementedException(); }
        }


        public TaskDistribution(DistributionOptimizer optimizer)
        {
            this.optimizer = optimizer;
        }

        public void Initialize(List<Worker> workers, List<Task> tasks, Int32 randomSeed)
        {
            
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
    }
}
