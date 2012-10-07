using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TaskOptimizer.Logging
{
    /// <summary>
    /// Basic implementation of Logger.
    /// Writes logs into a specified plain text file
    /// </summary>
    public class PlainTextLogger : Logger
    {
        StreamWriter _writer; 

        public PlainTextLogger(String filePath)
            : base()
        {
            _writer = new StreamWriter(filePath, true);
        }

        protected override void SubmitMessageQueue(LoggerEventArgs[] messages)
        {
            foreach (LoggerEventArgs e in messages)
                _writer.WriteLine("{0,-20}{1,-20}{2}", e.TimeStamp, e.Tag, e.Message);
            _writer.Flush();
        }

        public override void Dispose()
        {
            _writer.Flush();
            _writer.Close();
        }
    }
}
