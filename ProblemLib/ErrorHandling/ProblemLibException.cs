using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemLib.ErrorHandling
{
    /// <summary>
    /// Represents an Exception that is 
    /// </summary>
    public class ProblemLibException : Exception
    {
        public ProblemLibException(UInt32 errorCode, Exception innerException = null)
        {
            TimeStamp = DateTime.Now;
            this.ErrorCode = errorCode;
            this.InnerException = innerException;
        }

        /// <summary>
        /// Error code that is passed to remote end when
        /// this exception is caught
        /// </summary>
        public UInt32 ErrorCode { get; set; }
        /// <summary>
        /// Precise time when the exception was created.
        /// Used for finding related log entries.
        /// </summary>
        public DateTime TimeStamp { get; private set; }
        /// <summary>
        /// Exception wrapped into this exception (if any).
        /// Used for generating log entries.
        /// </summary>
        public Exception InnerException
        { get; private set; }

    }
}
