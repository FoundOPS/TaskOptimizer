using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace ProblemLib.Logging
{
    /// <summary>
    /// Abstract implementation of a logger.
    /// Inherit this class to create specific loggers
    /// </summary>
    public abstract class Logger : IDisposable
    {
        // Thread-safe queue for buffering 
        protected ConcurrentQueue<LoggerEventArgs> _messages = new ConcurrentQueue<LoggerEventArgs>();
        protected Thread _flushThread = null;  // Thread that takes care of monitoring message queue and submitting them to the log db
        protected Int32 _submissionBatch = 100;  // Specifies batch size (number of messages that trigger a submissin operation)

        // Indicates whether logger thread has been requested to stop
        protected Boolean _terminateRequest = false;

        // Properties
        /// <summary>
        /// Determines the number of messages written to the physical storage each submission.
        /// </summary>
        /// <remarks>
        /// Setting this property to a larger number will decrease the frequency of access to the underlying storage,
        /// which will improve performance if each access poses an overhead. However, in case of unexpected termination
        /// of the program, large submission batch size will result in last log entries being lost.
        /// </remarks>
        public Int32 SubmissionBatchSize
        {
            get { return _submissionBatch; }
            set
            {
                if (_flushThread.ThreadState == ThreadState.Unstarted)
                    _submissionBatch = value;
                else
                    throw new InvalidOperationException("Logger.set_SubmissionBatchSize(): Cannot set submission batch size once the logger thread starts");
            }
        }

        /// <summary>
        /// Constructor, initializes logger thread
        /// </summary>
        protected Logger()
        {
            ThreadStart flushStart = new ThreadStart(IterateLogger);
            _flushThread = new Thread(flushStart);
        }

        // Logger thread method, must remain private!
        private void IterateLogger()
        {
                while (true)
                {
                    if (!_terminateRequest)
                    {
                        // Do nothing if there is no need to flush messages
                        if (_messages.Count < _submissionBatch)
                            continue;
                        else
                        {
                            // Submit messages if needed

                            /* A faster way of doing this might be to copy entire _messages into an array via ToArray() and clearing the queue,
                             * but considering that there can be new messages arriving between these two instructions, copying one by one
                             * seems more reliable. More investigation needed.
                             */

                            LoggerEventArgs[] messages = new LoggerEventArgs[_submissionBatch];
                            for (int i = 0; i < _submissionBatch; i++)
                            {
                                if (!_messages.TryDequeue(out messages[i]))
                                    throw new Exception("Logger.RunLogger(): Failed to dequeue the next log message!");
                            }
                            SubmitMessageQueue(messages);
                        }
                    }
                    else
                    {
                        // If termination is requested, submit remaining messages and quit the thread
                        SubmitMessageQueue(_messages.ToArray());
                        ReleaseResources();
                        break;
                    }
                }
        }

        /// <summary>
        /// Event handler for OnLogMessage event
        /// </summary>
        /// <param name="sender">Event origin</param>
        /// <param name="e">Event data</param>
        public void HandleMessage(Object sender, LoggerEventArgs e)
        {
            EnqueueMessage(e);
        }

        /// <summary>
        /// Enqueues a message for processing
        /// </summary>
        /// <param name="arg"></param>
        public virtual void EnqueueMessage(LoggerEventArgs arg)
        {
            _messages.Enqueue(arg);
        }

        /// <summary>
        /// Starts the logger thread
        /// </summary>
        public void Run()
        {
            _flushThread.Start();
        }

        /// <summary>
        /// Requests the logger thread to stop.
        /// </summary>
        /// <param name="wait">Indicates whether to block caller thread until logger thread terminates</param>
        public void Stop(Boolean wait = true)
        {
            _terminateRequest = true;  // Set termination flag
            if (wait) _flushThread.Join();  // Wait for logger to quit
        }


        /// <summary>
        /// When implemented, writes a collection of log messages to underlying physical storage
        /// </summary>
        /// <param name="messages">Collection of messages to write</param>
        protected abstract void SubmitMessageQueue(LoggerEventArgs[] messages);

        /// <summary>
        /// Releases all resources used by logger.
        /// </summary>
        protected abstract void ReleaseResources();

        /// <summary>
        /// Member of IDisposable interface, do cleanup here.
        /// </summary>
        public abstract void Dispose();


    }

}
