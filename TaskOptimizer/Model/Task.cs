using TaskOptimizer.API;
using TaskOptimizer.Calculator;

namespace TaskOptimizer.Model
{
    public class Task
    {
        public Task(int id, double lat, double lon)
        {
            Id = id;
            Coordinate = new Coordinate(lat, lon);
        }

        /// <summary>
        /// Used for cloning a task
        /// </summary>
        /// <param name="task">The task to clone</param>
        public Task(Task task)
        {
            Id = task.Id;
            Coordinate = task.Coordinate;

            UserId = task.UserId;
            Time = task.Time;
            Problem = task.Problem;
        }

        public int UserId { get; set; }

        public int Id { get; private set; }

        public int Time { get; set; } // Seconds

        public Problem Problem { get; set; }

        public Coordinate Coordinate { get; set; }

        public int CostTo(Task task)
        {
            if (task == null || task == this)
            {
                //TODO add depot task, and return cost from here to there
                return 1;
            }

            if (Problem.GetCachedCost(this.Id, task.Id) == 0)
            {
                //cache the calculation in memory
                //_mDistances[task.Id] = Problem.Osrm.GetDistanceTime(new Coordinate(Lat, Lon), new Coordinate(task.Lat, task.Lon))[0];
                Problem.SetCachedCost(this.Id, task.Id, Problem.CostFunction.Calculate(this, task, true));
            }
            return Problem.GetCachedCost(this.Id, task.Id);
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
