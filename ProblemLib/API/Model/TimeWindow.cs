using System;

namespace ProblemLib.API.Model
{
    /// <summary>
    /// The span of time a job can be completed in. 
    /// Ex. From 3pm to 8pm
    /// </summary>
    public class TimeWindow
    {
        /// <summary>
        /// An inclusive start time (the time of day)
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// An inclusive end time (the time of day)
        /// </summary>
        public TimeSpan EndTime { get; set; }
    }
}