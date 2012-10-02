using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using ServiceStack.Redis;
using TaskOptimizer.API;

namespace TaskOptimizer.Calculator
{
    /// <summary>
    /// Interact with OSRM for map based calculations
    /// </summary>
    public class OSRM : IDisposable
    {
        // Factory Pattern
        private static Dictionary<Guid, OSRM> _sInstances = new Dictionary<Guid, OSRM>();

        public static OSRM GetInstance(Guid problemId)
        {
            if (!_sInstances.ContainsKey(problemId))
                _sInstances.Add(problemId, new OSRM());
            
            return _sInstances[problemId];
        }
        public static void ReleaseInstance(Guid problemId)
        {
            if (!_sInstances.ContainsKey(problemId)) return;
            _sInstances[problemId].Dispose();
            _sInstances.Remove(problemId);
        }


       
        // Static Variables
        private static readonly PooledRedisClientManager RedisClientManager = new PooledRedisClientManager(Constants.RedisServer);
        

        // TODO This is a hard coded set ID, need to change it to a parameter
        private const string SET_ID = "1$dt";

       // Instance Variables
        // Redis client forthis instance
        private IRedisClient mClient = RedisClientManager.GetClient();
        // Preprocessed entries cached in memory
        private Dictionary<String, int[]> mCachedEntries = new Dictionary<string, int[]>(); 

        /// <summary>
        /// Get the distance (in meters) and time (in seconds) between two coordinates
        /// </summary>
        /// <param name="a">First coordinate</param>
        /// <param name="b">Second coordinate</param>
        /// <returns>An array {distance, time}</returns>
        public int[] GetDistanceTime(Coordinate a, Coordinate b)
        {
            return GetDistanceTime_New(a, b);
            /*
            //meters
            int distance;
            //seconds
            int time;

            

            try
            {
                
                string lookupString = a.CompareTo(b) < 0
                                          ? a + "$" + b + "$dt"
                                          : b + "$" + a + "$dt";
                
                string distanceTime;

                using (var client = RedisClientManager.GetClient())
                {
                    //try to get the time and distance from the redis cache
                    distanceTime = client.Get<string>(lookupString);

                    //otherwise get it from OSRM
                    if (String.IsNullOrEmpty(distanceTime))
                    {
                        var route = CalculateRoute(a, b);
                        distanceTime = route.Route_Summary.Total_Distance + "$" + route.Route_Summary.Total_Time;

                        //if there is a redis client, cache the distance and time
                        client.SetEntry(lookupString, distanceTime);

                        client.Save();
                    }
                    else
                    {

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
                distance = GeoTools.StraightDistance(a.latRad, a.lonRad, b.latRad, b.lonRad);

                //TODO some calculation for time from a straight distance
                time = distance * 1;
            }


            return new[] { distance, time };
             */
        }

        private int[] GetDistanceTime_New(Coordinate a, Coordinate b)
        {
            int[] distanceTime = new int[2]; // distance (meters), time (seconds)

            try
            {
                string lookupString = a.CompareTo(b) < 0
                                          ? a + "$" + b
                                          : b + "$" + a;

                if (mCachedEntries.Count == 0)      // if local cache is empty, fetch data from redis
                {

                    HashSet<String> values = mClient.GetAllItemsFromSet(SET_ID);
                    foreach (String entry in values)
                    {
                        // CoordinateA$CoodinateB$distance$time
                        String[] splitEntry = entry.Split('$');
                        String key = splitEntry[0] + "$" + splitEntry[1];
                        mCachedEntries[key] = new int[] {Int32.Parse(splitEntry[2]), Int32.Parse(splitEntry[3])};
                    }
                }

                if (!mCachedEntries.ContainsKey(lookupString))      // if lookup string can't be found in cache
                {
                    // calculate route distance and time
                    var route = CalculateRoute(a, b);
                    distanceTime[0] = route.Route_Summary.Total_Distance;
                    distanceTime[1] = route.Route_Summary.Total_Time;

                    // put it into the local cache
                    mCachedEntries[lookupString] = distanceTime;

                    // put it into redis
                    String redisItem = String.Format("{0}${1}${2}", lookupString, distanceTime[0], distanceTime[1]);
                    mClient.AddItemToSet(SET_ID, redisItem);
                    //client.SaveAsync();
                    //client.Save();
                }
                
                // set up result
                distanceTime = mCachedEntries[lookupString];
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


        /// <summary>
        /// Calculate the route for two coordinates
        /// </summary>
        public OSMResponse CalculateRoute(Coordinate a, Coordinate b)
        {
            return CalculateRoute(new[] { a, b });
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
            var actionString = "nearest?loc=" + a.lat + "," + a.lon;
            var response = Request<LocResponse>(actionString);
            return new Coordinate(response.Mapped_Coordinate[0], response.Mapped_Coordinate[1]);
        }

        /// <summary>
        /// Make a request to the OSRM server
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON to</typeparam>
        /// <param name="action">Ex. viaroute?loc=124.124,151.222</param>
        private T Request<T>(string action) where T : class
        {
            var requestUrl = Constants.OSRMServer + action;

            var request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

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
            var requestUrl = Constants.OSRMServer + action;

            var request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                var result = sr.ReadToEnd();

                return result;
            }
        }

        //public static HttpWebResponse getRaw(string requestUrl)
        //{
        //    try
        //    {
        //        var request = WebRequest.Create(requestUrl) as HttpWebRequest;
        //        var response = request.GetResponse() as HttpWebResponse;
        //        if (response.StatusCode != HttpStatusCode.OK)
        //            throw new Exception(String.Format(
        //                "Server error (HTTP {0}: {1}).",
        //                response.StatusCode,
        //                response.StatusDescription));
        //        return response;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        #region IDIsposable

        public void Dispose()
        {
            mClient.Save();
            mClient.Dispose();
        }

        #endregion
    }
}
