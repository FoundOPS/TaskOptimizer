using ServiceStack.Redis;
using System;
using TaskOptimizer.API;

namespace TaskOptimizer.Calculator
{
    public static class Cost
    {
        /// <summary>
        /// The calculation used for cost based on distance and time
        /// </summary>
        /// <param name="distance">The distance in meters</param>
        /// <param name="time">The time in seconds</param>
        /// <param name="milesPerGallon">The truck's average MPG</param>
        /// <param name="gallonGas">The cost of a gallon of gas</param>
        /// <param name="hourlyWage">The average hourly wage of an employee</param>
        private static int Calculation(int distance, int time, int milesPerGallon = 10, int gallonGas = 4, int hourlyWage = 50)
        {
            //miles / milesPerGallon * gallonGas
            var distCost = ((distance / 1609.34) / milesPerGallon) * gallonGas;

            var timeCost = (time / 3600.0) * hourlyWage;
            var cost = (timeCost + distCost) * 100;

            return (int)cost;
        }
    }
}