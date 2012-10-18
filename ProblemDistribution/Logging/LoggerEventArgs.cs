using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProblemController.Logging
{
    public class LoggerEventArgs : EventArgs
    {
        public LoggerEventArgs(Int64 time, String tag, String message)
        {
            this.TimeStamp = time;
            this.Tag = tag;
            this.Message = message;
        }
        public LoggerEventArgs(String tag, String message)
            : this(DateTime.Now.Ticks / 10000, tag, message) { }
        public LoggerEventArgs(String tag, String format, params Object[] args)
            : this(tag, String.Format(format, args)) { }

        public Int64 TimeStamp { get; set; }
        public String Tag { get; set; }
        public String Message { get; set; }
    }
}
