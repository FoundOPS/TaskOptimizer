using TaskOptimizer.API;

namespace TaskOptimizer.Model
{
    public class Task
    {
        private readonly int[] m_distances;
        private readonly int m_id;
        private int m_defaultDistance;

        public Task(int id, int nbTasks)
        {
            m_id = id;
            m_distances = new int[nbTasks];
        }

        public Task(Task task)
        {
            m_id = task.Id;
            m_distances = task.m_distances;
            m_defaultDistance = task.m_defaultDistance;

            UserId = task.UserId;
            X = task.X;
            Y = task.Y;
            Effort = task.Effort;
        }

        public int UserId { get; set; }

        public int Id
        {
            get { return m_id; }
        }

        public double X { get; set; }

        public double Y { get; set; }

        public int Effort { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }

        public void setDefaultDistance(int distance)
        {
            m_defaultDistance = distance;
        }

        public void setDistance(Task task, int distance)
        {
            m_distances[task.Id] = distance;
        }

        public int distanceTo(Task task)
        {
            if (task == null || task == this)
            {
                return 0;
            }
            if (m_distances[task.Id] == 0)
            {
                m_distances[task.Id] = Precomp.getDistance(new Coordinate(X, Y), new Coordinate(task.X, task.Y));
            }
            return m_distances[task.Id];
        }

        public override string ToString()
        {
            return UserId.ToString();
        }

        public override bool Equals(object obj)
        {
            return ((Task) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}