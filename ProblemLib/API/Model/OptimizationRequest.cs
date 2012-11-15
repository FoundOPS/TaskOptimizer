using System;

namespace ProblemLib.API.Model
{
    public class OptimizationRequest
    {
        /// <summary>
        /// A unique Id for the request
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The dates a job can be done
        /// </summary>
        public DateWindow[] DateWindows { get; set; }

        /// <summary>
        /// The times a job can be done
        /// </summary>
        public TimeWindow[] TimeWindows { get; set; }

        /// <summary>
        /// The tasks to be completed
        /// </summary>
        public Task[] Tasks { get; set; }

        /// <summary>
        /// The workers to assign to routes
        /// </summary>
        public Resource[] Workers { get; set; }
    }
}
