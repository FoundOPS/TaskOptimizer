using System;

namespace ProblemLib.API
{
    public static class ControlCodes
    {
        #region Acknowledge and Error Reporting (0xD4)
        
        /// <summary>
        /// Represents an acknowledge message that indicates all data has
        /// been received without error.
        /// </summary>
        public const UInt16 Acknowledge = 0xD400;

        /// <summary>
        /// Requests remote machine to send data again.
        /// Used when critical data seems to be corrupted.
        /// </summary>
        public const UInt16 RequestResend = 0xD401;

        /// <summary>
        /// Represents an error message indicating that the data received caused
        /// an error. May be followed by an error message.
        /// </summary>
        public const UInt16 Error = 0xD4FF;

        #endregion

        #region Problem Distribution Command Codes (0xD5)

        /// <summary>
        /// Tells Distribution that configuration is going to be sent
        /// </summary>
        public const UInt16 SendingConfiguration = 0xD501;

        /// <summary>
        /// Tells Distribution to start preprocessing location data.
        /// Followed by parameters indicating alocated region for each distribution
        /// </summary>
        public const UInt16 StartPreprocessing = 0xD502;


        #endregion

        #region Connection Related Codes

        /// <summary>
        /// Tells remote end that connection is going to be terminated.
        /// Release all resources and prepare for stopping listener thread!
        /// </summary>
        public const UInt16 TerminateConnection = 0xD602;

        #endregion
    }
}
