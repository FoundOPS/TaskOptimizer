namespace ProblemLib.API.Model
{
    /// <summary>
    /// An ordered set of tasks assigned to a worker
    /// </summary>
    public class TaskSequence
    {
        /// <summary>
        /// Tasks in order
        /// </summary>
        Task[] OrderedTasks { get; set; }

        /// <summary>
        /// The assigned resources
        /// </summary>
        Resource[] Resources { get; set; }
    }

    /// <summary>
    /// An optimization solution
    /// </summary>
    public class Solution
    {
        public TaskSequence[] TaskSequences { get; set; }
    }
}