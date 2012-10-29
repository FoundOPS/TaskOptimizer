namespace ProblemLib.Interfaces
{
    public interface Individual
    {
        int Id { get; }
        int Fitness { get; }

        void Optimize();
        void Crossover(Individual parent1, Individual parent2);
        void Mutate();
    }
}