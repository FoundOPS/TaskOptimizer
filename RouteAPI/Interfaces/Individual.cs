namespace TaskOptimizer.Interfaces
{
    public interface Individual
    {
        int Id { get; }
        int Fitness { get; }

        void optimize();
        void crossover(Individual parent1, Individual parent2);
        void mutate();
    }
}