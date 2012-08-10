using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using ServiceStack.ServiceHost;
using TaskOptimizer;
using TaskOptimizer.API;
using System.Net;

namespace RouteAPI
{
    public class RouteAPI
    {
        public string Loc { get; set; }
    }
    public class APIResponse
    {
        public string Result { get; set; }
    }
    public class RouteAPIService : IService<RouteAPI>
    {
        public object Execute(RouteAPI request)
        {
            List<Coordinate> coords = new List<Coordinate>();
            String[] splitCoords = request.Loc.Split('$');
            foreach (String c in splitCoords)
            {
                String[] split = c.Split(',');
                coords.Add(new Coordinate(Double.Parse(split[0]), Double.Parse(split[1])));
            }
            HttpWebResponse resp = Precomp.getRawRoute(coords);
            String retString = "";
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            retString += sr.ReadToEnd();

            return retString;
        }
    } 
}
