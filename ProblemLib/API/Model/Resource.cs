using System;

namespace ProblemLib.API.Model
{
    public class Resource
    {
        /// <summary>
        /// The date/time window the resource is available.
        /// If null it is always available.
        /// </summary>
        public Window Availability { get; set; }

        /// <summary>
        /// Skills a resource has to complete a job
        /// </summary>
        public UInt32[] Skills { get; set; }
    }
}