using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using TaskOptimizer.API;

namespace TaskOptimizer.Calculator
{
    /// <summary>
    /// Interact with OSRM for map based calculations
    /// </summary>
    public static class OSRM
    {
        /// <summary>
        /// Calculate the route for two coordinates
        /// </summary>
        public static OSMResponse CalculateRoute(Coordinate a, Coordinate b)
        {
            return CalculateRoute(new[] { a, b });
        }

        private static string MakeRouteAction(IEnumerable<Coordinate> coords)
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
        public static OSMResponse CalculateRoute(ICollection<Coordinate> coords)
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
        public static string CalculateRouteRaw(ICollection<Coordinate> coords)
        {
            return RawRequest(MakeRouteAction(coords));
        }

        /// <summary>
        /// Find the nearest point on the map to a coordinate
        /// </summary>
        public static Coordinate FindNearest(Coordinate a)
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
        private static T Request<T>(string action) where T : class
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
        private static string RawRequest(string action)
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
    }
}
