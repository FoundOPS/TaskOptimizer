using System.Diagnostics;
using Mono.Unix;
using Mono.Unix.Native;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;
using TaskOptimizer.Model;
using Container = Funq.Container;

namespace TaskOptimizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
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
                for (bool exit = false; !exit; )
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
             * */

            int stopCount = 100;
            int truckCount = 10;

            if (args.Length == 2)
            {
                try
                {
                    stopCount = Int32.Parse(args[0]);
                    truckCount = Int32.Parse(args[1]);
                }
                catch
                {

                }
            }

            var sw = new Stopwatch();

            sw.Start();
            var problem = new Problem(new DefaultCost { MilesPerGallon = 10, PricePerGallon = 4, HourlyWage = 50 }, 2000);
            var tasks = Tools.GetTasks(Tools.GetCoordinates(stopCount), problem);
            var result = problem.Calculate(tasks, truckCount);
            sw.Stop();

            //Trace.WriteLine(String.Format("Total Time {0}ms", sw.ElapsedMilliseconds));
            Console.WriteLine("\nTotal Time {0}ms", sw.ElapsedMilliseconds);

            String test = result + " ";

            if (Debugger.IsAttached) // Disable waiting when not in debugging mode so that profiler gets more accurate results
                Console.ReadKey();
        }
    }

    public class RouteAPIHost : AppHostHttpListenerBase
    {
        //Tell Service Stack the name of your application and where to find your web services
        public RouteAPIHost()
            : base("Routing API", typeof(RouteAPIService).Assembly)
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

            var problem = new Problem(new DefaultCost {MilesPerGallon = 10, PricePerGallon = 4, HourlyWage = 50}, 1000);

            var tasks = Tools.GetTasks(coords, problem);

            var retString = problem.Calculate(tasks, numTrucks)[0];
            return retString;
        }
    }
}