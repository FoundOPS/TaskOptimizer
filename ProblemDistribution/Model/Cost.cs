using System;
using ProblemDistribution.Model;
using ProblemLib.API;
using ProblemLib.DataModel;

namespace ProblemDistribution.Model
{
    public interface ICostFunction
    {
        int Calculate(Task origin, Task destination, bool considerTaskTime);
    }

    class DefaultCost : ICostFunction
    {
        private DistributionOptimizer optimizer;

        public int MilesPerGallon { get; set; }
        public int PricePerGallon { get; set; }
        public int HourlyWage { get; set; }



        public DefaultCost(DistributionOptimizer optimizer)
        {
            this.optimizer = optimizer;
        }

        public int Calculate(Task origin, Task destination, bool considerTaskTime)
        {
            if (origin == null || destination == null || origin == destination)
                return 0;

            // Get time & Distance
          //  int[] distanceTime = optimizer.Osrm.GetDistanceTime(origin.Coordinates, destination.Coordinates);
            throw new NotImplementedException();
            /*
            // Calculate cost
            //miles / milesPerGallon * gallonGas
            var distCost = ((distanceTime[0] / 1609.34) / MilesPerGallon) * PricePerGallon;


            if (considerTaskTime)
                distanceTime[1] += (int)(origin.TimeOnTask + destination.TimeOnTask) / 2;

            var timeCost = (distanceTime[1] / 3600.0) * HourlyWage;
            var cost = (timeCost + distCost) * 100;

            return (int)cost;
             * */
        }
    }
}