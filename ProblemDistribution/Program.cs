#define USE_DEBUG_INNER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProblemDistribution.Utilities;
using ProblemLib.API;
using ProblemLib.Logging;

namespace ProblemDistribution
{
    class Program
    {
        // Basic structure for one problem distribution
        static void Main(string[] args)
        {
#if USE_DEBUG_INNER
            DebugInner();
            Console.ReadKey();
            return;
#endif
            ConsoleLogger console = new ConsoleLogger();
            GlobalLogger.AttachLogger(console);
            console.Run();

            DistributionServer server = new DistributionServer();
        }

        static void DebugInner()
        {
            Coordinate c0 = new Coordinate(1.234, 5.678);
            Coordinate c1 = new Coordinate(4.321, 8.765);
            Int32 d = 1234;
            Int32 t = 5678;

            String enc = RedisNumberEncoder.EncodeDistanceTime(c0, c1, d, t);
            Console.WriteLine(enc);

            Coordinate x0, x1;
            Int32 dist, time;
            RedisNumberEncoder.DecodeDistanceTime(enc, out x0, out x1, out dist, out time);
            Console.WriteLine("({0}, {1}) - ({2}, {3}) : D={4}, T={5}", x0.lat, x0.lon, x1.lat, x1.lon, dist, time);
        }
    }
}
