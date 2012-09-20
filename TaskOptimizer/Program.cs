using Mono.Unix;
using Mono.Unix.Native;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;
using Container = Funq.Container;

namespace TaskOptimizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Initialize app host
            try
            {
                var appHost = new RouteAPIHost();
                appHost.Init();
                appHost.Start("http://*:8081/");

                var signals = new[]
                    {
                        new UnixSignal(Signum.SIGINT),
                        new UnixSignal(Signum.SIGTERM),
                    };

                // Wait for a unix signal
                for (bool exit = false; !exit;)
                {
                    int id = UnixSignal.WaitAny(signals);

                    if (id >= 0 && id < signals.Length)
                    {
                        if (signals[id].IsSet) exit = true;
                    }
                }

                while (true)
                {
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    public class RouteAPIHost : AppHostHttpListenerBase
    {
        //Tell Service Stack the name of your application and where to find your web services
        public RouteAPIHost() : base("Routing API", typeof (RouteAPIService).Assembly)
        {
        }

        #region Overrides of HttpListenerBase

        public override void Configure(Container container)
        {
            //register user-defined REST-ful urls
            Routes
                .Add<RouteAPI>("/route")
                .Add<RouteAPI>("/route/{Loc}");
        }

        #endregion
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
            int numTrucks = 1;
            Console.WriteLine(request.Loc);
            var coords = new List<Coordinate>();
            String[] splitCoords = request.Loc.Split('$');
            try
            {
                int t = Int32.Parse(splitCoords[0]);
                if (t > 0) numTrucks = t;
            }
            catch
            {
            }
            foreach (String c in splitCoords)
            {
                try
                {
                    String[] split = c.Split(',');
                    coords.Add(new Coordinate(Double.Parse(split[0]), Double.Parse(split[1])));
                }
                catch
                {
                    continue;
                }
            }
            String retString = "";
            if (numTrucks == 1)
            {
                retString += Problem.getRawRoute(coords);
            }
            if (numTrucks > 1)
            {
                retString = Problem.getMultiRoute(coords, numTrucks);
            }
            return retString;
        }
    }
}