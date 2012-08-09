using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Drawing.Drawing2D;
using System.IO;
using TaskOptimizer.API;
using System.Net;
using System.Runtime.Serialization.Json;
using Npgsql;
using TaskOptimizer;
using TaskOptimizer.Model;
using TaskOptimizer.View;

namespace TaskOptimizer
{
    class Precomp
    {
        
        public static NpgsqlConnection pgsql = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=osmroutes;User Id=postgres;Password=14oaI29qa6up");
        public static void Main()
        {
            var s = getCSV(new FileInfo(@"C:\LatLon.csv"));
            DateTime startTime = System.DateTime.Now;
            Console.WriteLine(getDistance(s[0], s[1]));
    
            /*
            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
            sb.Host = "192.168.1.102";
            sb.UserName = "postgres";
            sb.Password = "14oaI29qa6up";
            sb.Database = "osmroutes";
            NpgsqlConnection pgsql = new NpgsqlConnection(sb.ConnectionString);
            pgsql.Open();
            NpgsqlCommand pgcmd = pgsql.CreateCommand();
           
            
            pgsql.Open();
             */
          List<Coordinate> stops = new List<Coordinate>();
                    Random r = new Random();
                    while (stops.Count < 100)
                    {
                        Coordinate c = s[r.Next(s.Count)];
                        if (!stops.Contains(c))
                        {
                            stops.Add(c);
                        }
                    }
                    
                    OSMResponse route = getRoute(stops);
                    foreach (OSMInstruction inst in route.Route_Instructions)
                    {
                        Console.WriteLine(inst.Road_Name+" " +inst.Instruction_Duration);
                    }
                    Console.WriteLine(DateTime.Now.Subtract(startTime).Ticks / 10000000.00);

                    Console.WriteLine(route.Route_Geometry);
        }
        public static void reCompute(ICollection<Coordinate> coords)
        {
            List<Coordinate> res_list = new List<Coordinate>();
            foreach (Coordinate c in coords)
            {
                Coordinate r = preClose(c);
                if (r == null) r = c;
                if (!res_list.Contains(r)) res_list.Add(r);
            }
            NpgsqlCommand pg_cmd = pgsql.CreateCommand();
            for (int i = 0; i < res_list.Count; i++)
            {
                String cmd_text = "INSERT INTO prepro VALUES ";
                for (int j = 0; j < res_list.Count; j++)
                {
                    if (i == j) continue;
                    RouteSummary rsum = MakeTransaction(res_list[i], res_list[j]).Route_Summary;
                    cmd_text += "('" + res_list[i].ToString() + "','" + res_list[j].ToString() + "'," + rsum.Total_Distance + "," + rsum.Total_Time + "),";
                }
                cmd_text = cmd_text.Substring(0, cmd_text.Length - 1) + ";";
                Console.WriteLine((i+1.0)/res_list.Count);
                pg_cmd.CommandText = cmd_text;
                try
                {
                    pg_cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }
        public static int preDist(Coordinate a, Coordinate b)
        {
            NpgsqlCommand pgcmd = pgsql.CreateCommand();

            var resolved_a = preClose(a);
            if (resolved_a == null) resolved_a = a;
            var resolved_b = preClose(b);
            if (resolved_b == null) resolved_b = b;
            pgcmd.CommandText = "SELECT length FROM prepro WHERE startpoint = '" + resolved_a.ToString() + "' AND endpoint = '"+resolved_b.ToString()+"';";
            var pgdata = pgcmd.ExecuteScalar();
            //Console.WriteLine(pgcmd.CommandText);
            //Console.WriteLine(resolved_a + ":" + resolved_b);
            //Console.WriteLine(Convert.ToInt32(pgdata));
            return Convert.ToInt32(pgdata);
        }
        public static Coordinate preClose(Coordinate a)
        {
            NpgsqlCommand pgcmd = pgsql.CreateCommand();
            pgcmd.CommandText = "SELECT resolved FROM closestnode WHERE raw='" + a.ToString() + "';";
            String s;
            try
            {
                s = pgcmd.ExecuteScalar() as string;
            }
            catch { return null; }
            if (s == null) return null;
            string[] coords = s.Split(',');
            return new Coordinate(double.Parse(coords[0]), double.Parse(coords[1]));
            
        }
        public static void speedTest(int max, int iterations)
        {
            var s = getCSV(new FileInfo(@"C:\LatLon.csv"));
            int numStops;
            for (numStops = 5; numStops <= max; numStops++)
            {
                double avgtime = 0.0;
                for (int i=0;i<iterations;i++){
                    List<Coordinate> stops = new List<Coordinate>();
                    Random r = new Random();
                    while (stops.Count < numStops)
                    {
                        Coordinate c = s[r.Next(s.Count)];
                        if (!stops.Contains(c))
                        {
                            stops.Add(c);
                        }
                    }
                    DateTime nowTime = DateTime.Now;
                    getRoute(stops);
                    avgtime += DateTime.Now.Subtract(nowTime).Ticks / 10000000;
                    }
                avgtime /= iterations;
                Console.WriteLine("Stops: "+numStops+" | Time: "+avgtime);
            }
        }
        public static List<Coordinate> getCSV(FileInfo f)
        {
            List<Coordinate> coords = new List<Coordinate>();
            StreamReader fs = f.OpenText();
            while (!fs.EndOfStream)
            {
                String s = fs.ReadLine();
                String[] points = s.Split(new[]{','}, 2);
                points[1] = points[1].Replace(",", "");
                Coordinate c = new Coordinate(double.Parse(points[0]), double.Parse(points[1]));
                if (!coords.Contains(c)) coords.Add(c);
            }
            return coords;
        }
        public static OSMResponse getRoute(List<Coordinate> stops)
        {
            List<Coordinate> resolved = new List<Coordinate>();
            foreach (Coordinate c in stops)
            {
                Coordinate r = closestPoint(c);
                if (!resolved.Contains(r))
                {
                    resolved.Add(r);
                }
            }
            List<Task> stopTasks = new List<Task>();
            foreach (Coordinate r in resolved)
            {
                Task t = new Task(resolved.IndexOf(r), resolved.Count);
                t.lat = r.lat;
                t.lon = r.lon;
                t.X = r.lat;
                t.Y = r.lon;
                t.Effort= 0;
                stopTasks.Add(t);
            }
            Optimizer.Configuration optConf = new Optimizer.Configuration();
            optConf.tasks = stopTasks;
            Robot truck = new Robot();
            optConf.robots = new List<Robot> { truck };
            optConf.nbDistributors = 1;
            optConf.progress = new nullProgress();
            optConf.randomSeed = 777777;
            FitnessLevels fl = new FitnessLevels();
            fl.CostMultiplier = 1;
            fl.TimeMultiplier=1;
            optConf.fitnessLevels = fl;
            optConf.startX = 37.222421;
            optConf.startY = -121.984476;
            Optimizer o = new Optimizer(optConf);
            System.Threading.Thread.Sleep(1000);
            
            while (o.stillInit() || o.MinSequences == null)
            {
                Console.WriteLine(o.m_creationThread.ThreadState.ToString());
            }
            Console.WriteLine(o.m_creationThread.ThreadState);
            List<Coordinate> routeList = new List<Coordinate>();
            foreach (Task t in o.MinSequences[0].Tasks)
            {
                Coordinate rp = new Coordinate(t.X, t.Y);
                routeList.Add(rp);
            }
            return MakeTransaction(makeRequest(routeList));
        }
        public static String makeRequest(ICollection<Coordinate> coords)
        {
            String requestString = "http://127.0.0.1:5050/viaroute?";
            foreach (Coordinate c in coords){
                requestString += "loc="+c.lat+","+c.lon+"&";
            }
            requestString = requestString.Substring(0, requestString.Length - 1);
            return requestString;
        }
        public static OSMResponse MakeTransaction(Coordinate a, Coordinate b)
        {
            return MakeTransaction(makeRequest(new[] { a, b }));
        }
        public static void updateClosest(ICollection<Coordinate> coords)
        {
            List<Coordinate> unique = new List<Coordinate>();
            foreach (Coordinate c in coords)
            {
                if (!unique.Contains(c)) unique.Add(c);
            }
            NpgsqlCommand pg_cmd = pgsql.CreateCommand();
            foreach (Coordinate c in unique)
            {
                Coordinate res_c = closestPoint(c);
                pg_cmd.CommandText = "INSERT INTO closestnode (raw,resolved) VALUES ('" + c.ToString() + "','" + res_c.ToString() + "');";
                Console.WriteLine(pg_cmd.CommandText);
               try
                {
                    pg_cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }
        public static Coordinate closestPoint(Coordinate a)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create("http://127.0.0.1:5050/nearest?loc="+a.lat+","+a.lon) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LocResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    LocResponse jsonResponse
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
        public static OSMResponse MakeTransaction(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(OSMResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    OSMResponse jsonResponse
                    = objResponse as OSMResponse;
                    List<OSMInstruction> instructions = new List<OSMInstruction>();
                    foreach (Object[] objs in jsonResponse.Raw_Route){
                        instructions.Add(new OSMInstruction(objs));
                    }
                    jsonResponse.Route_Instructions = instructions.ToArray();
                    List<AlternativeInstructions> alt_instructions = new List<AlternativeInstructions>();
                    foreach (Object[][] instSet in jsonResponse.Raw_Alternatives){
                        OSMInstruction[] i = new OSMInstruction[instSet.Length];
                        for (int j = 0; j < instSet.Length; j++)
                        {
                            i[j] = new OSMInstruction(instSet[j]);
                        }
                        AlternativeInstructions alt = new AlternativeInstructions();
                        alt.Instructions=i;
                        alt_instructions.Add(alt);
                    }
                    jsonResponse.Alternative_Instructions = alt_instructions.ToArray();
                    List<Coordinate> via_coords = new List<Coordinate>();
                    foreach (Object[] raw_coord in jsonResponse.Raw_Via){
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
        public static int getDistance(Coordinate a, Coordinate b)
        {
             try
            {
                HttpWebRequest request = WebRequest.Create("http://127.0.0.1:5050/distance?loc="+a.ToString()+"&loc="+b.ToString()) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    return Int32.Parse(sr.ReadToEnd());
                }
             }
                 catch (Exception e){
                     Console.WriteLine(e.Message);
                return 0;
                 
        }
        }
    }
    
}
