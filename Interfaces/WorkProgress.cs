namespace TaskOptimizer.Interfaces
{
    public interface WorkProgress
    {
        bool WorkCancelled { get; }
        void onWorkProgress(string description, int percent);

        void onWorkEnd();
    }
}