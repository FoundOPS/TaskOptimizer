using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ServiceStack.ServiceHost;
using TaskOptimizer.API;

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
        #region IService<RouteAPI> Members

        public object Execute(RouteAPI request)
        {
            var coords = new List<Coordinate>();
            String[] splitCoords = request.Loc.Split('$');
            foreach (String c in splitCoords)
            {
                String[] split = c.Split(',');
                coords.Add(new Coordinate(Double.Parse(split[0]), Double.Parse(split[1])));
            }
            HttpWebResponse resp = Precomp.getRawRoute(coords);
            String retString = "";
            var sr = new StreamReader(resp.GetResponseStream());
            retString += sr.ReadToEnd();

            return retString;
        }

        #endregion
    }
}