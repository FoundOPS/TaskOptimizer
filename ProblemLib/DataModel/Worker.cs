using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProblemLib.DataModel
{
    public class Worker :IEquatable<Worker>, ICloneable
    {
        public UInt32 WorkerID { get; set; }

        public Worker() { }

        /// <summary>
        /// Constructor.
        /// Equivalent to Worker.Clone()
        /// </summary>
        /// <param name="w"></param>
        public Worker(Worker w)
        {
            this.WorkerID = w.WorkerID;
        }

        /// <summary>
        /// Serializes contents of the Worker object to the stream.
        /// </summary>
        /// <param name="s">BinaryWriter for the specified stream</param>
        public void WriteToStream(BinaryWriter s)
        {
            s.Write(WorkerID);
        }
        /// <summary>
        /// Reads a Task object from a stream
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Worker ReadFromStream(BinaryReader r)
        {
            Worker w = new Worker();
            w.WorkerID = r.ReadUInt32();
            return w;
        }
        /// <summary>
        /// Returns the length of serialized data
        /// </summary>
        public static Int32 SerializedLength
        { get { return 4; } }

        #region Overrides

        public bool Equals(Worker other)
        {
            return this.WorkerID == other.WorkerID;
        }

        public override bool Equals(object obj)
        {
            if (obj is Worker) return this.WorkerID == ((Worker)obj).WorkerID;
            else return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)WorkerID;
        }

        public override string ToString()
        {
            return "Worker#" + WorkerID.ToString();
        }

        public object Clone()
        {
            return new Worker(this);
        }

        #endregion
    }
}
