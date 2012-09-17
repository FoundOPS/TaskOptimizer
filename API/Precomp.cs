using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using TaskOptimizer.Model;
using TaskOptimizer.View;
using ServiceStack.Redis;

namespace TaskOptimizer.API
{
    public class Precomp
    {
        static RedisClient rc = new RedisClient("127.0.0.1");
        public static HttpWebResponse getRawRoute(List<Coordinate> stops)
        {
            var resolved = new List<Coordinate>();
            foreach (Coordinate c in stops)
            {
                Coordinate r = closestPoint(c);
                if (!resolved.Contains(r))
                {
                    resolved.Add(r);
                }
            }
            var stopTasks = new List<Task>();
            foreach (Coordinate r in resolved)
            {
                var t = new Task(resolved.IndexOf(r), resolved.Count);
                t.lat = r.lat;
                t.lon = r.lon;
                t.X = r.lat;
                t.Y = r.lon;
                t.Effort = 0;
                stopTasks.Add(t);
            }
            var optConf = new Optimizer.Configuration();
            optConf.tasks = stopTasks;
            var truck = new Robot();
            optConf.robots = new List<Robot> {truck};
            optConf.randomSeed = 777777;
            var fl = new FitnessLevels();
            fl.CostMultiplier = 1;
            fl.TimeMultiplier = 1;
            optConf.fitnessLevels = fl;
            optConf.startX = optConf.tasks[0].lat;
            optConf.startY = optConf.tasks[0].lon;
            optConf.nbDistributors = Environment.ProcessorCount*3;
            var o = new Optimizer(optConf);
            while (o.m_minDistributor.m_nbIterationsWithoutImprovements < 10000) { }
            var routeList = new List<Coordinate>();

                foreach (Task t in o.MinSequences[0].Tasks)
                {
                    var rp = new Coordinate(t.X, t.Y);
                    routeList.Add(rp);
                }
            return getRaw(makeRequest(routeList));
        }
        public static String getMultiRoute(ICollection<Coordinate> coords, int numTrucks)
        {
            var resolved = new List<Coordinate>();
            foreach (Coordinate c in coords)
            {
                Coordinate r = closestPoint(c);
                if (!resolved.Contains(r))
                {
                    resolved.Add(r);
                }
            }
            var stopTasks = new List<Task>();
            foreach (Coordinate r in resolved)
            {
                var t = new Task(resolved.IndexOf(r), resolved.Count);
                t.lat = r.lat;
                t.lon = r.lon;
                t.X = r.lat;
                t.Y = r.lon;
                t.Effort = 0;
                stopTasks.Add(t);
            }
            var optConf = new Optimizer.Configuration();
            optConf.tasks = stopTasks;
            var truck = new Robot();
            optConf.robots = new List<Robot> ();
            for (int t = 0; t < numTrucks; t++)
            {
                optConf.robots.Add(truck);
            }
            optConf.randomSeed = 777777;
            var fl = new FitnessLevels();
            fl.CostMultiplier = 1;
            fl.TimeMultiplier = 100;
            optConf.fitnessLevels = fl;
            optConf.startX = optConf.tasks[0].lat;
            optConf.startY = optConf.tasks[0].lon;
            optConf.nbDistributors = Environment.ProcessorCount*3;
            var o = new Optimizer(optConf);
            while (o.m_minDistributor.m_nbIterationsWithoutImprovements<10000)
            {  }
            o.stop();
            var response = "{";
            int cont = 0;
            for (int r = 0; r < o.MinSequences.Count; r++)
            {
                if (o.MinSequences[r] == null)
                {
                    cont++;
                    continue;
                }
                Console.WriteLine(o.MinSequences[r].Tasks.Count);
                response += "\""+((r + 1) - cont)+"\"" + ": ";
                var routeList = new List<Coordinate>();

                foreach (Task t in o.MinSequences[r].Tasks)
                {
                    var rp = new Coordinate(t.X, t.Y);
                    routeList.Add(rp);
                }
                var sr = new StreamReader(getRaw(makeRequest(routeList)).GetResponseStream());               
                response += sr.ReadToEnd()+",";
            }
            response = response.Substring(0, response.Length - 1);
            return response+"}";
        }
        public static String makeRequest(ICollection<Coordinate> coords)
        {
            String requestString = "http://127.0.0.1:5050/viaroute?";
            foreach (Coordinate c in coords)
            {
                requestString += "loc=" + c.lat + "," + c.lon + "&";
            }
            requestString = requestString.Substring(0, requestString.Length - 1);
            return requestString;
        }

        public static OSMResponse MakeTransaction(Coordinate a, Coordinate b)
        {
            return MakeTransaction(makeRequest(new[] {a, b}));
        }

        public static Coordinate closestPoint(Coordinate a)
        {
            try
            {
                var request =
                    WebRequest.Create("http://127.0.0.1:5050/nearest?loc=" + a.lat + "," + a.lon) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var jsonSerializer = new DataContractJsonSerializer(typeof (LocResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    var jsonResponse
                        = objResponse as LocResponse;
                    return new Coordinate(jsonResponse.Mapped_Coordinate[0], jsonResponse.Mapped_Coordinate[1]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static HttpWebResponse getRaw(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                var response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                return response;
            }
            catch
            {
                return null;
            }
        }

        public static OSMResponse MakeTransaction(string requestUrl)
        {
            try
            {
                var request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var jsonSerializer = new DataContractJsonSerializer(typeof (OSMResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    var jsonResponse
                        = objResponse as OSMResponse;
                    var instructions = new List<OSMInstruction>();
                    foreach (var objs in jsonResponse.Raw_Route)
                    {
                        instructions.Add(new OSMInstruction(objs));
                    }
                    jsonResponse.Route_Instructions = instructions.ToArray();
                    var alt_instructions = new List<AlternativeInstructions>();
                    foreach (var instSet in jsonResponse.Raw_Alternatives)
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
                    jsonResponse.Alternative_Instructions = alt_instructions.ToArray();
                    var via_coords = new List<Coordinate>();
                    foreach (var raw_coord in jsonResponse.Raw_Via)
                    {
                        via_coords.Add(new Coordinate(Convert.ToDouble(raw_coord[0]), Convert.ToDouble(raw_coord[1])));
                    }
                    jsonResponse.Via_Points = via_coords.ToArray();
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static OSMResponse getRoute(List<Coordinate> coords)
        {
            return MakeTransaction(getRawRoute(coords));
        }

        public static OSMResponse MakeTransaction(HttpWebResponse response)
        {
            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                var jsonSerializer = new DataContractJsonSerializer(typeof (OSMResponse));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                var jsonResponse
                    = objResponse as OSMResponse;
                var instructions = new List<OSMInstruction>();
                foreach (var objs in jsonResponse.Raw_Route)
                {
                    instructions.Add(new OSMInstruction(objs));
                }
                jsonResponse.Route_Instructions = instructions.ToArray();
                var alt_instructions = new List<AlternativeInstructions>();
                foreach (var instSet in jsonResponse.Raw_Alternatives)
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
                jsonResponse.Alternative_Instructions = alt_instructions.ToArray();
                var via_coords = new List<Coordinate>();
                foreach (var raw_coord in jsonResponse.Raw_Via)
                {
                    via_coords.Add(new Coordinate(Convert.ToDouble(raw_coord[0]), Convert.ToDouble(raw_coord[1])));
                }
                jsonResponse.Via_Points = via_coords.ToArray();
                return jsonResponse;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static int getDistance(Coordinate a, Coordinate b)
        {
            var lookupString = a.ToString() + "$" + b.ToString();
            if (rc.Exists(lookupString)>0)
            {
                return Int32.Parse(rc.GetValue(lookupString));
            }
            try
            {
                
                var request = WebRequest.Create("http://127.0.0.1:5050/distance?loc=" + a + "&loc=" + b) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var sr = new StreamReader(response.GetResponseStream());
                    var distance = Int32.Parse(sr.ReadToEnd());
                    rc.SetEntry(lookupString, distance + "");
                    return distance;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public static int getCost(Coordinate a, Coordinate b)
        {
            var lookupString = a.ToString() + "$" + b.ToString() + "$cost";
            if (rc.Exists(lookupString) > 0)
            {
                return Int32.Parse(rc.GetValue(lookupString));
            }
            var cost = getCost(MakeTransaction(a, b));
            rc.SetEntry(lookupString, (int)cost + "");
            return cost;
        }
        public static int getCost(OSMResponse routeInfo)
        {
            var timeCost = (routeInfo.Route_Summary.Total_Time / 90.00);
            var distCost = (routeInfo.Route_Summary.Total_Distance /1287.00);
            var cost = (timeCost + distCost) * 100;
            return (int)cost;
        }
    }
}