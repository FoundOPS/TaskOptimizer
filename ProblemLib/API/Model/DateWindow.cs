using System;

namespace ProblemLib.API.Model
{
    /// <summary>
    /// The span of dates a job can be completed in.
    /// Ex. From 1-1-2012 to 1-4-2012
    /// </summary>
    public class DateWindow
    {
        /// <summary>
        /// An inclusive start date
        /// </summary>
        public TimeSpan StartDate { get; set; }

        /// <summary>
        /// An inclusive end date
        /// </summary>
        public TimeSpan EndDate { get; set; }
    }
}