using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using TaskOptimizer.API;
using TaskOptimizer.Calculator;
using TaskOptimizer.Logging;
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

            CommandLineArguments cargs = new CommandLineArguments();

            int stopCount = 100;
            int truckCount = 4;
            String config = null;

            for (int i = 0; i < cargs.Count; i++)
            {
                CommandLineArgument arg = cargs[i];
                if (arg.Type == CommandLineArgument.ArgumentType.Option)
                {
                    switch (arg.Name.ToLower())
                    {
                        case "stops":
                            stopCount = Int32.Parse(arg.Arguments[0]);
                            break;
                        case "trucks":
                            truckCount = Int32.Parse(arg.Arguments[0]);
                            break;
                        case "config":
                            config = arg.Arguments[0];
                            break;
                    }
                }
            }

            if (config != null) Configuration.Initialize(config);
            else Configuration.Initialize();


            var sw = new Stopwatch();
           
            using (var logger = new PlainTextLogger(Path.Combine(Configuration.Instance.RootDirectory, "TaskOptimizer.log")))
            {
                ConsoleLogger console = ConsoleLogger.Instance;

                sw.Start();
                var problem = new Problem(new DefaultCost { MilesPerGallon = 10, PricePerGallon = 4, HourlyWage = 50 }, 1000);
                problem.AttachLogger(logger);
                problem.AttachLogger(console);
                logger.Run();
                console.Run();

                var tasks = Tools.GetTasks(Tools.GetCoordinates(stopCount), problem);
                var result = problem.Calculate(tasks, truckCount);
                sw.Stop();

                logger.Stop(false);
                console.Stop(false);
                console.HandleMessage(problem, new LoggerEventArgs("TaskOptimizer", "Total Time: {0}ms", sw.ElapsedMilliseconds));

                //String test = result + " ";
            }

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