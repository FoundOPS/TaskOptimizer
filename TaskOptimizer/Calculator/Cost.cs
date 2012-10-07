using TaskOptimizer.Model;

namespace TaskOptimizer.Calculator
{
    public interface ICostFunction
    {
        int Calculate(Task origin, Task destination, bool considerTaskTime);
    }

    public class DefaultCost : ICostFunction
    {
        public int MilesPerGallon { get; set; }
        public int PricePerGallon { get; set; }
        public int HourlyWage { get; set; }

        public int Calculate(Task origin, Task destination, bool considerTaskTime)
        {
            if (origin == null || destination == null || origin == destination)
                return 0;

#if DEBUG
            if (origin.Problem != destination.Problem)
                throw new System.Exception("Tasks have to belong to the same problem!");
#endif
            // Get time & Distance
            int[] distanceTime = origin.Problem.Osrm.GetDistanceTime(origin.Coordinate, destination.Coordinate);
            
            // Calculate cost
            //miles / milesPerGallon * gallonGas
            var distCost = ((distanceTime[0] / 1609.34) / MilesPerGallon) * PricePerGallon;


            if (considerTaskTime)
                distanceTime[1] += (origin.Time + destination.Time)/2;

            var timeCost = (distanceTime[1] / 3600.0) * HourlyWage;
            var cost = (timeCost + distCost) * 100;

            return (int)cost;
        }
    }
}