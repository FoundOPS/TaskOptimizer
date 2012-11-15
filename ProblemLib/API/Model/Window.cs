namespace ProblemLib.API.Model
{
    /// <summary>
    /// A limiting date/time window
    /// </summary>
    public class Window
    {
        /// <summary>
        /// The times it can be completed/is available
        /// If null there is no time restriction
        /// </summary>
        public TimeWindow[] TimeWindow { get; set; }

        /// <summary>
        /// The dates it can be completed/is available
        /// If null there is no date restriction
        /// </summary>
        public DateWindow[] DateWindow { get; set; }
    }
}