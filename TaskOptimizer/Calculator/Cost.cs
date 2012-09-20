using ServiceStack.Redis;
using System;
using TaskOptimizer.API;

namespace TaskOptimizer.Calculator
{
    public static class Cost
    {
        /// <summary>
        /// The calculation used for cost based on distance and time.
        /// TODO: make this configurable
        /// </summary>
        private static int Calculation(int distance, int time)
        {
            double distCost = (distance / 1287.00);
            double timeCost = (time / 90.00);
            double cost = (timeCost + distCost) * 100;
            return (int)cost;
        }

        /// <summary>
        /// Calculate the cost between two coordinates based on distance and time
        /// </summary>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <param name="redisClient">(Optional) The redis client for caching the result</param>
        /// <returns>The cost</returns>
        public static int Calculate(Coordinate a, Coordinate b, IRedisClient redisClient = null)
        {
            int distance;
            int time;

            try
            {
                var lookupString = a + "$" + b + "$dt";

                string distanceTime;

                //try to get the time and distance from the redis cache
                if (redisClient != null && redisClient.ContainsKey(lookupString))
                {
                    distanceTime = redisClient.GetValue(lookupString);
                }
                //otherwise get it from OSRM
                else
                {
                    var route = OSRM.CalculateRoute(a, b);
                    distanceTime = route.Route_Summary.Total_Distance + "$" + route.Route_Summary.Total_Time;

                    //cache if there is a redis client
                    if (redisClient != null)
                    {
                        redisClient.SetEntry(lookupString, distanceTime);
                    }
                }

                var values = distanceTime.Split('$');
                distance = Int32.Parse(values[0]);
                time = Int32.Parse(values[1]);
            }
            //if there is a problem, use straight distance, fake time calculation
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                distance = StraightDistance(a, b);
                //TODO some calculation for time from a straight distance
                time = distance * 1;
            }

            return Calculation(distance, time);
        }

        #region Othodromic Distance from https://github.com/lmaslanka/Orthodromic-Distance-Calculator

        /// <summary>
        /// Calculate the straight distance between two points
        /// </summary>
        private static int StraightDistance(Coordinate a, Coordinate b)
        {
            return (int)VincentyDistanceFormula(a.lat, a.lon, b.lat, b.lon);
        }

        /// <summary>
        /// Calculate the arc length distance between two locations using the Vincenty formula
        /// </summary>
        /// <returns>
        /// The distance in meters
        /// </returns>
        private static double VincentyDistanceFormula(double aLat, double aLon, double bLat, double bLon)
        {
            var aLatRad = DegreeToRadian(aLat);
            var aLonRad = DegreeToRadian(aLon);

            var bLatRad = DegreeToRadian(bLat);
            var bLonRad = DegreeToRadian(bLon);

            return Math.Atan(Math.Sqrt(
                ((Math.Pow(Math.Cos(bLatRad) * Math.Sin(Diff(aLonRad, bLonRad)), 2)) +
                (Math.Pow((Math.Cos(aLatRad) * Math.Sin(bLatRad)) - (Math.Sin(aLatRad) * Math.Cos(bLatRad) * Math.Cos(Diff(aLonRad, bLonRad))), 2)))
                /
                ((Math.Sin(aLatRad) * Math.Sin(bLatRad)) +
                (Math.Cos(aLatRad) * Math.Cos(bLatRad) * Math.Cos(Diff(aLonRad, bLonRad))))));
        }

        private static double Diff(double a, double b)
        {
            return Math.Abs(b - a);
        }

        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        #endregion
    }
}
