using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProblemLib.Logging
{
    /// <summary>
    /// Basic implementation of Logger.
    /// Writes logs into a specified plain text file
    /// </summary>
    public class PlainTextLogger : Logger
    {
        StreamWriter _writer;

        public PlainTextLogger(String filePath, Boolean overwrite = true)
            : base()
        {
            _writer = new StreamWriter(filePath, !overwrite);
        }

        protected override void SubmitMessageQueue(LoggerEventArgs[] messages)
        {
            foreach (LoggerEventArgs e in messages)
            {
                DateTime dt = new DateTime(e.TimeStamp * 10000);
                String time = String.Format("{0}:{1}:{2}.{3}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
                _writer.WriteLine("{0,-20}{1,-20}{2}", time, e.Tag, e.Message);
            }
            _writer.Flush();
        }

        public override void Dispose()
        {
        }

        protected override void ReleaseResources()
        {
            _writer.Close();
        }
    }
}
