using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemLib.DataModel;
using ProblemLib.Interfaces;

namespace ProblemDistribution.Model
{
    class TaskSequence : Individual
    {

        private List<Task> tasks;

        public List<Task> Tasks
        {
            get { return tasks; }
            set
            {
                tasks = value;
                throw new NotImplementedException();
            }
        }

        public Worker Worker { get; set; }


        public TaskSequence(DistributionOptimizer optimizer)
        {

        }

        #region Members of Individual

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

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
    }
}
