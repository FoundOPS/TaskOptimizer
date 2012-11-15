using System;

namespace ProblemLib.API.Model
{
    public enum Stage
    {
        Queued,
        Preprocessing,
        Processing,
        Completed
    }

    public class OptimizationResponse
    {
        /// <summary>
        /// 1-100 percent progress of current stage
        /// (Only set in preprocessing stage)
        /// </summary>
        public UInt16? Progress { get; set; }

        /// <summary>
        /// The stage the request is in
        /// </summary>
        public Stage Stage { get; set; }

        /// <summary>
        /// The solution.
        /// Null unless the stage is completed
        /// </summary>
        public Solution Solution { get; set; }
    }
}