using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemController.Logging
{
    /// <summary>
    /// Static class that all global loggers attach to
    /// </summary>
    public static class GlobalLogger
    {
        private static EventHandler<LoggerEventArgs> _onLogMessage;
        public static event EventHandler<LoggerEventArgs> OnLogMessage
        {
            add { _onLogMessage += value; }
            remove { _onLogMessage -= value; }
        }
        /// <summary>
        /// Sends a log message to all attached loggers
        /// </summary>
        /// <param name="tag">Tag associated with the message</param>
        /// <param name="format">Format string of the message</param>
        /// <param name="args">Message format arguments</param>
        internal static void SendLogMessage(String tag, String format, params Object[] args)
        {
            if (_onLogMessage != null)
            {
                LoggerEventArgs e = new LoggerEventArgs(tag, format, args);
                _onLogMessage(null, e);
            }
        }

        /// <summary>
        /// Attaches a logger to the global logger
        /// </summary>
        /// <param name="logger"></param>
        public static void AttachLogger(Logger logger)
        {
            OnLogMessage += logger.HandleMessage;
        }
        /// <summary>
        /// Detaches a logger from the global logger
        /// </summary>
        /// <param name="logger"></param>
        public static void DetachLogger(Logger logger)
        {
            OnLogMessage -= logger.HandleMessage;
        }
    }
}
