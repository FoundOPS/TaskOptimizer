using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System.Net;
using TaskOptimizer;
using TaskOptimizer.API;
using Mono.Unix;
using Mono.Unix.Native;

namespace RouteAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
              //Initialize app host
            var appHost = new RouteAPIHost();
            appHost.Init();
            appHost.Start("http://*:8081/");

            UnixSignal[] signals = new UnixSignal[] { 
				new UnixSignal(Signum.SIGINT), 
				new UnixSignal(Signum.SIGTERM), 
			};

            // Wait for a unix signal
            for (bool exit = false; !exit; )
            {
                int id = UnixSignal.WaitAny(signals);

                if (id >= 0 && id < signals.Length)
                {
                    if (signals[id].IsSet) exit = true;
                }
            }
        }
    }
    public class RouteAPIHost : AppHostHttpListenerBase
    {
        //Tell Service Stack the name of your application and where to find your web services
        public RouteAPIHost() : base("Routing API", typeof(RouteAPIService).Assembly) { }

        public override void Configure(Funq.Container container)
        {
            //register user-defined REST-ful urls
            Routes
              .Add<RouteAPI>("/route")
              .Add<RouteAPI>("/route/{Loc}");
        }
    }
    public class RouteAPI
    {
        public string Loc { get; set; }
    }
    public class APIResponse
    {
        public string Result { get; set; }
    }
    public class RouteAPIService : ServiceBase<RouteAPI>
    {
        protected override object Run(RouteAPI request)
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
