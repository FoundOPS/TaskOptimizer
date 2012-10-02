using System;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;

namespace TaskOptimizer.Model
{
    public class Task
    {
        private readonly int[] _mDistances;
        private readonly int[] _mStraightDistances;

        public Task(int id, int nbTasks)
        {
            Id = id;
            _mDistances = new int[nbTasks];
            _mStraightDistances = new int[nbTasks];
        }

        /// <summary>
        /// Used for cloning a task
        /// </summary>
        /// <param name="task">The task to clone</param>
        public Task(Task task)
        {
            Id = task.Id;
            _mDistances = task._mDistances;
            _mStraightDistances = task._mStraightDistances;
            Lat = task.Lat;
            Lon = task.Lon;

            UserId = task.UserId;
            Effort = task.Effort;
        }

        public int UserId { get; set; }

        public int Id { get; private set; }

        public int Effort { get; set; }

        public Guid ProblemId { get; internal set; }

        private double _lat;
        public double Lat
        {
            get { return _lat; }
            set
            {
                _lat = value;
                LatRad = GeoTools.DegreeToRadian(_lat);
            }
        }

        private double _lon;
        public double Lon
        {
            get { return _lon; }
            set
            {
                _lon = value;
                LonRad = GeoTools.DegreeToRadian(_lon);
            }
        }

        public double LatRad { get; private set; }
        public double LonRad { get; private set; }

        public int StraightDistanceTo(Task task)
        {
            if (task == null || task == this)
                return 0;

            var straightDistance = _mStraightDistances[task.Id];
            if (straightDistance == 0)
            {
                straightDistance = GeoTools.StraightDistance(LatRad, LonRad, task.LatRad, task.LonRad);
                //cache the calculation
                _mStraightDistances[task.Id] = straightDistance;
            }

            return straightDistance;
        }
        public int DistanceTo(Task task)
        {
            if (task == null || task == this)
            {
                return 0;
            }

            if (_mDistances[task.Id] == 0)
            {
                //cache the calculation in memory
                _mDistances[task.Id] = OSRM.GetInstance(ProblemId).GetDistanceTime(new Coordinate(Lat, Lon), new Coordinate(task.Lat, task.Lon))[0];
            }
            return _mDistances[task.Id];
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(object obj)
        {
            return ((Task)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}