using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using ProblemLib.ErrorHandling;
using ProblemLib.Utilities;
using ServiceStack.Redis;

namespace ProblemLib.API
{
    /// <summary>
    /// Interact with OSRM for map based calculations
    /// </summary>
    public class Osrm : IDisposable
    {
        // Factory Pattern
        private static readonly Dictionary<Guid, Osrm> SInstances = new Dictionary<Guid, Osrm>();

        public static Osrm GetInstance(Guid problemId, String redisServer, String osrmServer)
        {
            if (!SInstances.ContainsKey(problemId))
                SInstances.Add(problemId, new Osrm(redisServer, osrmServer));

            return SInstances[problemId];
        }
        public static void ReleaseInstance(Guid problemId)
        {
            if (!SInstances.ContainsKey(problemId)) return;
            SInstances[problemId].Dispose();
            SInstances.Remove(problemId);
        }
        
        // TODO This is a hard coded set ID, need to change it to a parameter
        private const string SetIdDistancetime = "1$dt";
        private const string SetIdNearestlocation = "1$nl";

        // Instance Variables
        // Redis client forthis instance
        private String _redisServer;
        private String _osrmServer;
        private readonly IRedisClient _mClient;
        // Preprocessed entries cached in memory
        private readonly Dictionary<String, int[]> _mCachedEntries = new Dictionary<string, int[]>();
        private readonly Dictionary<String, Coordinate> _mCachedLocations = new Dictionary<String, Coordinate>();
        private readonly PooledRedisClientManager _mClientManager;

        // Constructor
        public Osrm(String redisServer, String osrmServer)
        {
            _redisServer = redisServer;
            _osrmServer = osrmServer;
            _mClientManager = new PooledRedisClientManager(redisServer);
            _mClient = _mClientManager.GetClient();
        }

        public void PullCache()
        {
            HashSet<String> values;

            try
            {
                // Get all cached distance/time entries
                values = _mClient.GetAllItemsFromSet(SetIdDistancetime);
            }
            catch (RedisException x)
            {
                throw new ProblemLibException(ErrorCodes.RedisConnectionFailed, x);
            }

            foreach (String entry in values)
            {
                // CoordinateA$CoodinateB$distance$time
                String[] splitEntry = entry.Split('$');
                String key = splitEntry[0] + "$" + splitEntry[1];
                _mCachedEntries[key] = new int[] { Int32.Parse(splitEntry[2]), Int32.Parse(splitEntry[3]) };
            }

            // Get all cached coordinate/nearestLocation entries
            values = _mClient.GetAllItemsFromSet(SetIdNearestlocation);
            foreach (String entry in values)
            {
                // Coordinate$NearestLocation
                // Coordinates rounded to 5 decimal places
                String[] splitEntry = entry.Split(new char[] { '$' });
                String original = splitEntry[0];
                String[] splitCoordinate = splitEntry[1].Split(new char[] { ',' });
                Coordinate resolved = new Coordinate(Double.Parse(splitCoordinate[0]), Double.Parse(splitCoordinate[1]));
                _mCachedLocations[original] = resolved;
            }
        }

        /// <summary>
        /// Get the distance (in meters) and time (in seconds) between two coordinates
        /// </summary>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <returns>An array {distance, time}</returns>
        public int[] GetDistanceTime(Coordinate a, Coordinate b)
        {
            int[] distanceTime = new int[2]; // distance (meters), time (seconds)

            try
            {
                string lookupString = a.CompareTo(b) < 0
                                          ? a + "$" + b
                                          : b + "$" + a;

                if (_mCachedEntries.Count == 0)      // if local cache is empty, fetch data from redis
                {
                    HashSet<String> values = _mClient.GetAllItemsFromSet(SetIdDistancetime);
                    foreach (String entry in values)
                    {
                        // CoordinateA$CoodinateB$distance$time
                        String[] splitEntry = entry.Split('$');
                        String key = splitEntry[0] + "$" + splitEntry[1];
                        _mCachedEntries[key] = new int[] { Int32.Parse(splitEntry[2]), Int32.Parse(splitEntry[3]) };
                    }
                }

                if (!_mCachedEntries.ContainsKey(lookupString))      // if lookup string can't be found in cache
                {
                    // calculate route distance and time
                    var route = CalculateRoute(a, b);
                    distanceTime[0] = route.Route_Summary.Total_Distance;
                    distanceTime[1] = route.Route_Summary.Total_Time;

                    // put it into the local cache
                    _mCachedEntries[lookupString] = distanceTime;

                    // put it into redis
                    String redisItem = String.Format("{0}${1}${2}", lookupString, distanceTime[0], distanceTime[1]);
                    _mClient.AddItemToSet(SetIdDistancetime, redisItem);
                    //client.SaveAsync();
                    //client.Save();
                }

                // set up result
                distanceTime = _mCachedEntries[lookupString];
            }
            //if there is a problem, use straight distance, fake time calculation
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                distanceTime[0] = GeoTools.StraightDistance(a.latRad, a.lonRad, b.latRad, b.lonRad);

                //TODO some calculation for time from a straight distance
                distanceTime[1] = distanceTime[0] * 1;

                //TODO log lat/long
            }

            return distanceTime;
        }

        private string MakeRouteAction(IEnumerable<Coordinate> coords)
        {
            var actionString = "viaroute?";
            foreach (Coordinate c in coords)
                actionString += "loc=" + c.lat + "," + c.lon + "&";
            //remove the last &
            actionString = actionString.Substring(0, actionString.Length - 1);
            return actionString;
        }

        /// <summary>
        /// Calculate the route for two coordinates
        /// </summary>
        public OSMResponse CalculateRoute(Coordinate a, Coordinate b)
        {
            return CalculateRoute(new[] { a, b });
        }

        /// <summary>
        /// Calculate the route for a set of coordinates
        /// </summary>
        public OSMResponse CalculateRoute(ICollection<Coordinate> coords)
        {
            var response = Request<OSMResponse>(MakeRouteAction(coords));

            //fixup the response
            var instructions = new List<OSMInstruction>();
            foreach (var objs in response.Raw_Route)
                instructions.Add(new OSMInstruction(objs));

            response.Route_Instructions = instructions.ToArray();
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
            var via_coords = new List<Coordinate>();
            foreach (var raw_coord in response.Raw_Via)
                via_coords.Add(new Coordinate(Convert.ToDouble(raw_coord[0]), Convert.ToDouble(raw_coord[1])));

            response.Via_Points = via_coords.ToArray();
            return response;
        }

        /// <summary>
        /// Same as CalculateRoute but returns the JSON string
        /// </summary>
        /// <returns>A string</returns>
        public string CalculateRouteRaw(ICollection<Coordinate> coords)
        {
            return RawRequest(MakeRouteAction(coords));
        }

        /// <summary>
        /// Find the nearest point on the map to a coordinate
        /// </summary>
        public Coordinate FindNearest(Coordinate a)
        {
            // round the original
            a.lat = Math.Round(a.lat, 5);
            a.lon = Math.Round(a.lon, 5);

            // try find the location in cache
            if (_mCachedLocations.ContainsKey(a.ToString()))
                return _mCachedLocations[a.ToString()];

            var actionString = "nearest?loc=" + a.lat + "," + a.lon;
            var response = Request<LocResponse>(actionString);
            Coordinate result = new Coordinate(response.Mapped_Coordinate[0], response.Mapped_Coordinate[1]);

            // cache result
            _mCachedLocations[a.ToString()] = result;
            _mClient.AddItemToSet(SetIdNearestlocation, String.Format("{0}${1}", a, result));

            return result;
        }

        /// <summary>
        /// Make a request to the OSRM server
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON to</typeparam>
        /// <param name="action">Ex. viaroute?loc=124.124,151.222</param>
        private T Request<T>(string action) where T : class
        {
            var requestUrl = _osrmServer + action;

            var request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ProblemLibException(ErrorCodes.OsrmConnectionFailed, new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription)));

                var jsonSerializer = new DataContractJsonSerializer(typeof(T));
                var objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                return objResponse as T;
            }
        }

        /// <summary>
        /// Make a request to the OSRM server and get a string back
        /// </summary>
        /// <param name="action">Ex. viaroute?loc=124.124,151.222</param>
        private string RawRequest(string action)
        {
            var requestUrl = _osrmServer + action;

            var request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new ProblemLibException(ErrorCodes.OsrmConnectionFailed, new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription)));

                var sr = new StreamReader(response.GetResponseStream());
                var result = sr.ReadToEnd();

                return result;
            }
        }

        #region IDIsposable

        public void Dispose()
        {
            _mClient.Save();
            _mClient.Dispose();
        }

        #endregion

        #region Utility Methods (Caching)



        #endregion
    }
}
