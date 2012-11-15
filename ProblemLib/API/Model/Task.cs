using System;

namespace ProblemLib.API.Model
{
    /// <summary>
    /// Class representing a single task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Id of the task
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Latitude component of the task location
        /// </summary>
        public Decimal Latitude { get; set; }

        /// <summary>
        /// Longitude component of the task location
        /// </summary>
        public Decimal Longitude { get; set; }

        /// <summary>
        /// Time required to complete the task in seconds
        /// </summary>
        public UInt32 TimeOnTask { get; set; }

        /// <summary>
        /// The dollar value of completing a task
        /// </summary>
        public UInt32 Value { get; set; }

        /// <summary>
        /// TODO: FUTURE Priority (1)
        /// The restrictive date/time window the task can be completed
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// TODO: FUTURE Priority (2)
        /// Required skills to complete a job
        /// If null there are no required skills
        /// </summary>
        public UInt32[] RequiredSkills { get; set; }
    }
}