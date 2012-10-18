using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemController.Logging
{
    /// <summary>
    /// Logger implementation that writes log messages to the console.
    /// </summary>
    public class ConsoleLogger : Logger
    {
        // Maxumum number of characters on the same line
        private readonly Int32 _splitPoint = Console.BufferWidth - 35;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConsoleLogger()
        {
            // Sincenwriting stuff to console doesn't really take any resources
                // set submission batch to 1
            _submissionBatch = 1;
        }

        protected override void SubmitMessageQueue(LoggerEventArgs[] messages)
        {
            // Just in case if there are many mesages...
            foreach (LoggerEventArgs e in messages)
            {
                String message = e.Message;
                // If message fits in one line then print it as it is
                if (message.Length < _splitPoint)
                    WriteMessageLine(e.TimeStamp, e.Tag, e.Message);
                else
                {
                    // Message too long, split it into multiple lines
                    Boolean firstLine = true;
                    while (message.Length != 0)
                    {
                        Int32 split = message.Substring(0, message.Length > _splitPoint ? _splitPoint : message.Length).LastIndexOf(' ');
                        if (split < 0) split = message.Length > _splitPoint ? _splitPoint : message.Length;
                        String msgLine = message.Substring(0, split);
                        message = message.Substring(message.Length > split + 1 ? split + 1 : message.Length);
                        if (firstLine)
                        {
                            WriteMessageLine(e.TimeStamp, e.Tag, msgLine);
                            firstLine = false;
                        }
                        else
                            WriteMessageLine(null, null, msgLine);
                    }
                }
            }
        }

        protected override void ReleaseResources()
        {
        }

        public override void Dispose()
        {
        }

        private void WriteMessageLine(Int64? timeStamp, String tag, String message)
        {
            String time = null;
            if (timeStamp.HasValue)
            {
                DateTime dt = new DateTime(timeStamp.Value * 10000);
                time = String.Format("{0}:{1}:{2}.{3}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            }

            Console.WriteLine("{0,-15}{1,-20}{2}", time != null ? time : "", tag != null ? tag : "", message);
        }
    }
}
