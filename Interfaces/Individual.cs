using System;
using System.Collections.Generic;
using System.Text;

namespace TaskOptimizer.Interfaces
{
    public interface Individual
    {
        int Id { get;}

        void optimize();
        void crossover(Individual parent1, Individual parent2);
        void mutate();

        int Fitness { get;}
    }
}
