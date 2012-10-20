using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace ProblemLib.DataModel
{
    /// <summary>
    /// Class representing a single task
    /// </summary>
    /// <remarks>
    /// In order to simplify the AI and avoid transmitting useless data over network,
    /// only barebone structure is kept. Data such as user id should be kept in the 
    /// database server.
    /// </remarks>
    public class Task : IEquatable<Task>, ICloneable
    {
        /// <summary>
        /// ID of the task
        /// </summary>
        public UInt32 TaskID { get; set; }
        /// <summary>
        /// Longitude component of the task location
        /// </summary>
        public Single Longitude { get; set; }
        /// <summary>
        /// Latitude component of the task location
        /// </summary>
        public Single Latitude { get; set; }
        /// <summary>
        /// Estimated time required to complete the task
        /// </summary>
        public UInt32 TimeOnTask { get; set; }
        /// <summary>
        /// Determines whether the task can be rearranged within the sequence
        /// </summary>
        public Boolean CanRearrange { get; set; }

        /// <summary>
        /// Constructor
        /// Only to be used from within the Task class
        /// </summary>
        private Task() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID of the task</param>
        /// <param name="lon">Longitude component of the task location</param>
        /// <param name="lat">Latitude component of the task location</param>
        public Task(UInt32 id, Single lon, Single lat)
        {
            this.TaskID = id;
            this.Longitude = lon;
            this.Latitude = lat;
        }
        /// <summary>
        /// Constructor for cloning tasks.
        /// Equivalent to Task.Clone()
        /// </summary>
        /// <param name="o">Task to be cloned</param>
        public Task(Task o)
        {
            this.TaskID = o.TaskID;
            this.Longitude = o.Longitude;
            this.Latitude = o.Latitude;
            this.TimeOnTask = o.TimeOnTask;
            this.CanRearrange = o.CanRearrange;
        }

        /// <summary>
        /// Serializes contents of the Task object to the stream.
        /// </summary>
        /// <param name="s">BinaryWriter for the specified stream</param>
        public void WriteToStream(BinaryWriter s)
        {
            s.Write(TaskID);
            s.Write(Longitude);
            s.Write(Latitude);
            s.Write(TimeOnTask);
            s.Write(CanRearrange);
        }
        /// <summary>
        /// Reads a Task object from a stream
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Task ReadFromStream(BinaryReader r)
        {
            Task t = new Task();
            t.TaskID = r.ReadUInt32();
            t.Longitude = r.ReadSingle();
            t.Latitude = r.ReadSingle();
            t.TimeOnTask = r.ReadUInt32();
            t.CanRearrange = r.ReadBoolean();
            return t;
        }
        /// <summary>
        /// Returns the length of serialized data
        /// </summary>
        public static Int32 SerializedLength
        { get { return 17; } }

        #region Overrides

        public bool Equals(Task other)
        {
            return this.TaskID == other.TaskID;
        }

        public override bool Equals(object obj)
        {
            Task t = obj as Task;
            return t != null && t.TaskID == this.TaskID;
        }

        public override int GetHashCode()
        {
            return (int)TaskID;
        }

        public override string ToString()
        {
            return this.TaskID.ToString();
        }

        public object Clone()
        {
            return new Task(this);
        }
        
        #endregion

    }
}
