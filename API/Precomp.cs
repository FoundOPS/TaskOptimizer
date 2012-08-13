using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using TaskOptimizer.Model;
using TaskOptimizer.View;

namespace TaskOptimizer.API
{
    public class Precomp
    {
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
            optConf.nbDistributors = 1;
            optConf.progress = new nullProgress();
            optConf.randomSeed = 777777;
            var fl = new FitnessLevels();
            fl.CostMultiplier = 1;
            fl.TimeMultiplier = 1;
            optConf.fitnessLevels = fl;
            optConf.startX = 37.222421;
            optConf.startY = -121.984476;
            var o = new Optimizer(optConf);
            Thread.Sleep(1000);

            while (o.stillInit() || o.MinSequences == null)
            {
                Console.WriteLine(o.m_creationThread.ThreadState.ToString());
            }
            Console.WriteLine(o.m_creationThread.ThreadState);
            var routeList = new List<Coordinate>();
            foreach (Task t in o.MinSequences[0].Tasks)
            {
                var rp = new Coordinate(t.X, t.Y);
                routeList.Add(rp);
            }
            return getRaw(makeRequest(routeList));
        }

        public static String makeRequest(ICollection<Coordinate> coords)
        {
            String requestString = "http://192.168.2.102:5050/viaroute?";
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
                    WebRequest.Create("http://192.168.2.102:5050/nearest?loc=" + a.lat + "," + a.lon) as HttpWebRequest;
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
            try
            {
                var request =
                    WebRequest.Create("http://192.168.2.102:5050/distance?loc=" + a + "&loc=" + b) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    var sr = new StreamReader(response.GetResponseStream());
                    return Int32.Parse(sr.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}