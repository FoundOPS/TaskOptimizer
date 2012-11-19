using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using ProblemLib.Logging;
using ProblemLib.Preprocessing;
using ProblemLib.Utilities;

namespace ProblemLib.API
{
    /// <summary>
    /// Uncached OSRM APIs.
    /// </summary>
    /// <remarks>
    /// This is only a wrapper for OSRM API. This class does not
    /// contain any form of caching logic.
    /// </remarks>
    public static class OsrmAPI
    {
        /// <summary>
        /// Get the distance (in meters) and time (in seconds) between two coordinates
        /// </summary>
        /// <param name="osrmAddress">Address of the OSRM server</param>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <returns>Pair(distance, time)</returns>
        public static Pair<Int32, Int32> GetDistanceTime(String osrmAddress, Coordinate a, Coordinate b)
        {
            // Distance (meters), Time (seconds)
            Pair<Int32, Int32> distanceTime = new Pair<Int32, Int32>(-1, -1);

            try
            {
                var route = CalculateRoute(osrmAddress, a, b);
                distanceTime.First = route.Route_Summary.Total_Distance;
                distanceTime.Second = route.Route_Summary.Total_Time;
            }
            catch (Exception ex)
            {
                throw new ProblemLib.ErrorHandling.ProblemLibException(
                    ErrorCodes.RedisConnectionFailed, ex);
            }

            return distanceTime;
        }
        /// <summary>
        /// Finds nearest OSRM node.
        /// </summary>
        /// <param name="osrmAddress">Address of the OSRM server</param>
        /// <param name="c">Coordinate</param>
        /// <returns></returns>
        public static Coordinate FindNearestNode(String osrmAddress, Coordinate c)
        {
            String actionString = "nearest?loc=" + c.lat + "," + c.lon;
            LocResponse response = NetworkUtilities.JsonRequest<LocResponse>(
                new Uri(osrmAddress + actionString));

            if (response.Mapped_Coordinate.Length < 2)
            {
                GlobalLogger.SendLogMessage("OSRM_Error", "Coordinate out of bounds ({0}, {1})", c.First, c.Second);
                return new Coordinate(0, 0);
            }
            return new Coordinate(response.Mapped_Coordinate[0], response.Mapped_Coordinate[1]);

        }

        /// <summary>
        /// Calculate the route for two coordinates
        /// </summary>
        public static OSMResponse CalculateRoute(String osrmAddress, Coordinate a, Coordinate b)
        {
            return CalculateRoute(osrmAddress, new[] { a, b });
        }
        /// <summary>
        /// Calculate the route for a set of coordinates
        /// </summary>
        public static OSMResponse CalculateRoute(String osrmAddress, IEnumerable<Coordinate> coords)
        {
            // Get initial response
            var response = NetworkUtilities.JsonRequest<OSMResponse>(
                new Uri(osrmAddress + MakeRouteAction(coords)));

            // process response
                // here i used a linq expression instead of foreach loop
                // this part is dependent on System.Linq namespace!
            response.Route_Instructions = (
                from objs in response.Raw_Route select new OSMInstruction(objs)).ToArray();

            var alt_instructions = new List<AlternativeInstructions>();
            foreach (var instSet in response.Raw_Alternatives)
            {
                var i = new OSMInstruction[instSet.Length];
                for (int j = 0; j < instSet.Length; j++)
                {
                    i[j] = new OSMInstruction(instSet[j]);
                }
                var alt = new AlternativeInstructions();
                alt.Instructions = i;
                alt_instructions.Add(alt);
            }
            response.Alternative_Instructions = alt_instructions.ToArray();

            // another linq shorthand
            response.Via_Points = (from raw_coord in response.Raw_Via
                                   select
                                       new Coordinate(Convert.ToDouble(raw_coord[0]), Convert.ToDouble(raw_coord[1]))).ToArray();

            return response;
        }
        /// <summary>
        /// Calculates the route for a collection of coordinates but does not parse the response.
        /// </summary>
        /// <param name="osrmAddress">Address of the OSRM server</param>
        /// <param name="coords">Collection of coordinates</param>
        /// <returns></returns>
        public static String CalculateRouteRaw(String osrmAddress, IEnumerable<Coordinate> coords)
        {
            return NetworkUtilities.Request(new Uri(osrmAddress + MakeRouteAction(coords)));
        }

        /// <summary>
        /// Converts coordinate collection into OSRM request string.
        /// </summary>
        /// <param name="coords">Collection of coordinates</param>
        /// <returns></returns>
        private static String MakeRouteAction(IEnumerable<Coordinate> coords)
        {
            StringBuilder actionString = new StringBuilder("viaroute?");
            foreach (Coordinate c in coords)
                actionString.AppendFormat("loc={0},{1}&", c.lat, c.lon);
            actionString.Remove(actionString.Length - 1, 1);
            return actionString.ToString();
        }
    }
}
