namespace TaskOptimizer.Model
{
    public class Worker
    {
        public Worker()
        {
            DistanceCost = 1;
            WorkCost = 1;
            DistanceTime = 1;
            WorkTime = 1;
        }

        public int DistanceCost { get; set; }
        public int WorkCost { get; set; }

        public int DistanceTime { get; set; }
        public int WorkTime { get; set; }
    }
}